using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace ArcadeBridge
{
    public class CarMovement : MonoBehaviour
    {
        public CarParkingSystem ParkingSystem;
        public CarParkPoint AssignedParkPoint;
        
        public CarSpawningSystem _carSpawningSystem;
        
        public Transform nextCarTransform = null;
        public Transform finalStopPoint = null;
        public Transform despawnPoint = null;
        
        public int stoppingDistanceFromNextCar = 0, stopPointInd = 0, timer = 0;
        
        public bool reachedStopPoint = false, ParkWaitTimeCompleted = false;
        public bool reachedParkingSpot = false, assignedAParkPoint = false;


        [SerializeField] private NavMeshAgent _navMeshAgent;
        private float distance = 0, initialSpeed = 3.5f, speedIncrementFactor = 0.01f;
        private bool MoveAcrossNavMeshesStarted = false;

        ParkPointLocation ParkPointLoc = ParkPointLocation.baseLevel;

        private void OnEnable()
        {
            CarParkPoint.OnSpaceAvailable += OnCarSpaceAvailable;
            ParkingSystem.ticketingSystem.OnTicketAvailable += OnTicketsAvailable;
        }

        private void OnDisable()
        {
            CarParkPoint.OnSpaceAvailable -= OnCarSpaceAvailable;
            ParkingSystem.ticketingSystem.OnTicketAvailable -= OnTicketsAvailable;
        }

        // Start is called before the first frame update
        void Start()
        {
            initialSpeed = _navMeshAgent.speed;
            _navMeshAgent.SetDestination(finalStopPoint.position);
        }

        public void SetNewParkingSpot(Transform point)
        {
            _navMeshAgent.SetDestination(point.position);
            assignedAParkPoint = true;
        }

        public void SetCarData(Cars carData)
        {
            gameObject.transform.SetPositionAndRotation(new Vector3(carData.carPosition.pos.x,carData.carPosition.pos.y,carData.carPosition.pos.z),
                Quaternion.Euler(new Vector3(carData.carPosition.rot.x,carData.carPosition.rot.y,carData.carPosition.rot.z)));

            timer = carData.timeAssigned;
            
            initialSpeed = carData.initialSpeed;
            _navMeshAgent.speed = initialSpeed;
            
            _navMeshAgent.SetDestination(new Vector3(carData.carPosition.pos.x,carData.carPosition.pos.y,carData.carPosition.pos.z));
            
            //Assign ParkPoint
            AssignedParkPoint = ParkingSystem.FindSpecificParkPoint(carData.assignedParkPoint.floorID,
                carData.assignedParkPoint.parkingPointNo);
            
            //Call the SetParkPointData function from the assigned parkpoint 
            AssignedParkPoint.SetParkPointData(carData.assignedParkPoint,this);
        }

        public Cars GetCarData()
        {
            Cars car = new Cars();

            var transform1 = transform;
            car.carPosition.SetPosVector(transform1.position);;
            car.carPosition.SetRotVector(transform1.rotation.eulerAngles); 
            car.carPosition.SetScaleVector(transform1.localScale);

            car.destination.SetPosVector(_navMeshAgent.destination);

            car.initialSpeed = initialSpeed;

            car.assignedParkPoint = AssignedParkPoint.GetParkPointData();

            car.timeAssigned = timer;
            car.floorID = AssignedParkPoint.FloorID;
            return car;
        }
        
        public void ResetSpeed()
        {
            _navMeshAgent.speed = initialSpeed;
        }

        public void DespawnTime()
        {
            StartCoroutine(MoveToDespawnPoint());
        }

        public void WaitTimeCompleted()
        {
            StartCoroutine(CanGoDownTheLift());
        }
        
        private IEnumerator CanGoDownTheLift()
        {
            yield return new WaitForSecondsRealtime(timer);
            
            ParkWaitTimeCompleted = true;
            //The list which keeps tracks of cars entering and exiting the parking system
            ParkingSystem.carsInParkingLot.Remove(this);
            
            ParkingSystem.AddToOutputQueue.Invoke(this);

            //The output lift should check for cars here!
            ParkingSystem.lift.OutputLiftHandling();
        }
        
        private IEnumerator MoveToDespawnPoint()
        {
            yield return new WaitForSecondsRealtime(timer);
            ParkWaitTimeCompleted = true;
            Debug.Log("Car " + gameObject.name + " moving towards despawn point and assigned park point is null" + (AssignedParkPoint == null));
            if (AssignedParkPoint != null)
            {
                ParkingSystem.AddToAvailablePointsQueue(AssignedParkPoint);
                AssignedParkPoint.Car = null;
                AssignedParkPoint.isSpaceAvailable = true;
                AssignedParkPoint = null;
            }
            SetNewDestination(ParkingSystem.parkingSystemOutputPoint);
            ParkingSystem.AddToCarsToLeaveParkingLotQueue(this);
            StartCoroutine(OffMeshLinkMovement());
        }
 
        IEnumerator MoveAcrossNavMeshLink()
        {
            Vector3 previousDestination = _navMeshAgent.destination;
            
            OffMeshLinkData data = _navMeshAgent.currentOffMeshLinkData;
            //_navMeshAgent.updateRotation = false;
 
            Vector3 startPos = _navMeshAgent.transform.position;
            Vector3 endPos = data.endPos + Vector3.up * _navMeshAgent.baseOffset;
            float agentVelocity = 4f;
            float duration = (endPos-startPos).magnitude/agentVelocity;
            float t = 0.0f;
            float tStep = 1.0f/duration;
            while(t<1.0f)
            {
                transform.rotation = Quaternion.Lerp(_navMeshAgent.transform.rotation,
                    _navMeshAgent.currentOffMeshLinkData.offMeshLink.endTransform.rotation, t);
                transform.position = Vector3.Lerp(startPos,endPos,t);
                _navMeshAgent.destination = transform.position;
                t+=tStep*Time.deltaTime;
                yield return null;
            }
            transform.position = endPos;
            //_navMeshAgent.updateRotation = true;
            _navMeshAgent.CompleteOffMeshLink();
            _navMeshAgent.SetDestination(previousDestination);
            MoveAcrossNavMeshesStarted= false;
        }

        IEnumerator OffMeshLinkMovement()
        {
            yield return new WaitUntil(() => _navMeshAgent.isOnOffMeshLink && !MoveAcrossNavMeshesStarted);
            
            //Debug.Log("Smoothing out the offmesh link!!");
            MoveAcrossNavMeshesStarted=true;
            StartCoroutine(MoveAcrossNavMeshLink());
        }
        private void Update()
        {
            // if(_navMeshAgent !=  null)
            //     Debug.Log("off mesh "+_navMeshAgent.isOnOffMeshLink);
            if (nextCarTransform != null)
            {
                distance = Vector3.Distance(transform.position, nextCarTransform.position);
                //Debug.Log("The car: " + gameObject.name + "has a distance from next Car: " + distance);
                if (distance <= stoppingDistanceFromNextCar)
                {
                    //Debug.Log("The car: " + gameObject.name + "\nDistance is less than stopping Distance! " + distance);
                    // ReSharper disable once PossibleLossOfFraction
                    _navMeshAgent.speed -= (stoppingDistanceFromNextCar / 4);
                }
                else if(distance > stoppingDistanceFromNextCar && _navMeshAgent.speed < initialSpeed)
                {
                    _navMeshAgent.speed += speedIncrementFactor;
                }
            }
            
            //We have reached the stop point
            if (!ParkWaitTimeCompleted && !reachedStopPoint && _navMeshAgent.remainingDistance < 0.5f 
                && ParkingSystem.ticketingSystem.ticketsAvailable)
            {
                reachedStopPoint = true;
                CarSpotAssignment();
            }
        }
        
        public void CarReachedDropOffArea()
        {
            _navMeshAgent.enabled = true;
            transform.SetParent(_carSpawningSystem.spawnPoint.transform);
            SetNewDestination(ParkingSystem.parkingSystemOutputPoint);
            ParkingSystem.AddToCarsToLeaveParkingLotQueue(this);
            StartCoroutine(OffMeshLinkMovement());
        }
        
        public void CarAssignedTo_OutLift()
        {
            ParkingSystem.RemoveFromOutputQueue?.Invoke(this);
            
            _navMeshAgent.destination = Vector3.zero;
            _navMeshAgent.isStopped = true;
            _navMeshAgent.enabled = false;
            
            Debug.Log("Reaching the OUT-Lift!");
            
            //Make the Car a child of the In_Lift GameObject to move it on the lift
            transform.SetParent(ParkingSystem.lift.OUT_LiftPad.transform);

            ParkingSystem.lift.Move_OutLift_ToBaseLevel();
        }
        
        public void CarAssignedTo_InLift()
        {
            _navMeshAgent.destination = Vector3.zero;
            _navMeshAgent.isStopped = true;
            _navMeshAgent.enabled = false;
//            Debug.Log("Reached the In-Lift!");
            
            //Make the Car a child of the In_Lift GameObject to move it on the lift
            transform.SetParent(ParkingSystem.lift.IN_LiftPad.transform);
            
            //Move Lift To designated Floor
            ParkingSystem.lift.LiftMovement(AssignedParkPoint.FloorID,LiftType.IN);
        }

        public void MoveToPointOnFloor()
        {
            _navMeshAgent.enabled = true;
            ParkingSystem.lift.RemoveSpecificItemFromList(this);
            transform.SetParent(_carSpawningSystem.spawnPoint.transform);
            SetNewParkingSpot(AssignedParkPoint.waypoint);
        }
        
        void OnCarSpaceAvailable()
        {
            if (!ParkWaitTimeCompleted && reachedStopPoint && ParkingSystem.ticketingSystem.ticketsAvailable && AssignedParkPoint == null)
            {
                Debug.Log("Parking Space Available! " + gameObject.name);
                CarSpotAssignment();
            }
        }

        void OnTicketsAvailable()
        {
            if (!ParkWaitTimeCompleted && reachedStopPoint && AssignedParkPoint == null)
            {
                Debug.Log("Car Spot Assignment! " + gameObject.name);
                CarSpotAssignment();
            }
        }
        
        void CarSpotAssignment()
        {
            bool? isParkingSpotAvailable = ParkingLotsHandler.AssignAParkingLane?.Invoke(gameObject);
            
            //If parking spot available
            if (isParkingSpotAvailable != null && isParkingSpotAvailable == true)
            {
                Debug.Log("parking spot available! " + gameObject.name);
                ParkingSystem.ticketingSystem.TicketUsed?.Invoke();
                
                //Dequeue from car spawning list
                CarSpawningSystem.dequeueFromList?.Invoke(gameObject);
//                Debug.Log("Dequeued Car From list!");
                
                if (AssignedParkPoint.FloorID >= 1)
                {
                    ParkPointLoc = ParkPointLocation.higherLevel;
                }
                
                StartCoroutine(OffMeshLinkMovement());
                if(ParkPointLoc == ParkPointLocation.baseLevel)
                {
                    //Set the destination to the parking spot
                    SetNewParkingSpot(AssignedParkPoint.waypoint);
                }
                else
                {
                    //Add the car to Lift input queue
                    ParkingSystem.lift.InputLiftQueueHandling(this);
                }
                ParkingSystem.carsInParkingLot.Add(this);
            }
        }
        
        public void SetNewDestination(Transform point)
        {
            _navMeshAgent.SetDestination(point.position);
        }
        
        
        
        // private IEnumerator ParkingSpotMovement()
        // {
        //     //Set the destination to the parking spot
        //     SetNewParkingSpot(AssignedParkPoint.waypoint);
        //     StartCoroutine(OffMeshLinkMovement());
        //     //Debug.Log("Set Parking spot!");
        //     
        //     yield return new WaitUntil(() =>
        //         reachedStopPoint && !reachedParkingSpot && assignedAParkPoint && !_navMeshAgent.isOnOffMeshLink && 
        //         _navMeshAgent.remainingDistance < 0.01f);
        //     reachedParkingSpot = true;
        //     
        //     //Debug.Log("Reached Parking Spot!");
        //     StartCoroutine(MoveToDespawnPoint());
        // }
        // private IEnumerator ParkingSpotAssignment()
        // {
        //     //Debug.Log("The car " + gameObject.name + " has reached the stop point!");
        //     //Are there tickets available at the booth
        //     yield return new WaitUntil(() => ParkingSystem.ticketingSystem.ticketsAvailable);
        //     
        //     //Is a parking spot available?
        //     bool? isParkingSpotAvailable = ParkingLotsHandler.AssignAParkingLane?.Invoke(gameObject);
        //     
        //     //If parking spot available
        //     if (isParkingSpotAvailable != null && isParkingSpotAvailable == true)
        //     {
        //         ParkingSystem.ticketingSystem.TicketUsed?.Invoke();
        //         //Dequeue from car spawning list
        //         CarSpawningSystem.dequeueFromList?.Invoke(gameObject);
        //         Debug.Log("Dequeued Car From list!");
        //
        //         Transform point = null;
        //         if (AssignedParkPoint.FloorID < 1)
        //         {
        //             point = AssignedParkPoint.waypoint;
        //         }
        //         else
        //         {
        //             point = ParkingSystem.lift.IN_LiftPad.transform;
        //         }
        //         
        //         //Set the destination to the parking spot
        //         SetNewParkingSpot(point);
        //         StartCoroutine(OffMeshLinkMovement());
        //         //Debug.Log("Set Parking spot!");
        //
        //         yield return new WaitForSecondsRealtime(1);
        //         
        //         yield return new WaitUntil(() =>
        //             reachedStopPoint && !reachedParkingSpot && assignedAParkPoint && !_navMeshAgent.isOnOffMeshLink 
        //             && !MoveAcrossNavMeshesStarted && _navMeshAgent.remainingDistance < 0.01f);
        //         
        //         //Car is supposed to move on the base floor
        //         if (AssignedParkPoint.FloorID < 1)
        //         {
        //             reachedParkingSpot = true;
        //             StartCoroutine(MoveToDespawnPoint());
        //         }
        //         //Car has to move to a specific level
        //         else
        //         {
        //             // if(ParkingSystem.lift.CarOnLift)
        //             //     ParkingSystem.lift.HandleCarQueue(this);
        //             
        //             //Move the car to the lift and to the parking spot
        //             StartCoroutine(MoveToLift());
        //         }
        //     }
        // }
        // IEnumerator MoveToLift()
        // {
        //     yield return new WaitUntil(() => !ParkingSystem.lift.CarOnInLift);
        //     
        //     ParkingSystem.lift.CarOnInLift = true;
        //     
        //     _navMeshAgent.destination = Vector3.zero;
        //     _navMeshAgent.isStopped = true;
        //     _navMeshAgent.enabled = false;
        //     Debug.Log("Reached the In-Lift!");
        //     
        //     //Make the Car a child of the In_Lift GameObject to move it on the lift
        //     transform.SetParent(ParkingSystem.lift.IN_LiftPad.transform);
        //     Debug.Log("Changed Parent to : In-Lift!");
        //
        //     yield return new WaitForSecondsRealtime(1f);
        //     
        //     //Move the Lift to assigned level
        //     ParkingSystem.lift.LiftMovement(AssignedParkPoint.FloorID,LiftType.IN);
        //     Debug.Log("Moved the Lift to assigned level!");
        //     
        //     //Wait until the lift has reached destination level
        //     yield return new WaitUntil(() => ParkingSystem.lift.ReachedDestinationLevel_InLift);
        //     Debug.Log("Lift Reached Destination Level!");
        //     
        //     _carSpawningSystem = ParkingLotsHandler.GetSpawningSystemReference?.Invoke();
        //     
        //     if(_carSpawningSystem != null)
        //         //Un-child the Car 
        //         transform.SetParent(_carSpawningSystem.spawnPoint.transform);
        //     else
        //         Debug.LogError("Couldn't un-child the car since no spawning system reference available");
        //     
        //     _navMeshAgent.enabled = true;
        //     
        //     //Set the parking point to be the destination of the car
        //     _navMeshAgent.SetDestination(AssignedParkPoint.waypoint.position);
        //
        //     ParkingSystem.lift.InputCarsQueue.Remove(this);
        //     yield return new WaitForSecondsRealtime(1);
        //     
        //     //Return back the lift to its original position
        //     ParkingSystem.lift.Move_InLift_Down();
        // }
    }
}

public enum ParkPointLocation
{
    baseLevel,
    higherLevel
}