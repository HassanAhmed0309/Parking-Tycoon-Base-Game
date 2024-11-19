using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.Serialization;

namespace ArcadeBridge
{
    public class LiftController : MonoBehaviour
    {
        [SerializeField] public GameObject IN_LiftPad;
        [SerializeField] public GameObject OUT_LiftPad;
        [SerializeField] public BoxCollider IN_LiftPad_Collider;
        [SerializeField] public BoxCollider OUT_LiftPad_Collider;
        [SerializeField] private CarParkingSystem parking;
        
        public List<GameObject> LevelStopPoints;
        public List<CarMovement> InputCarsQueue;
        public bool ReachedDestinationLevel_InLift = false, ReachedDestinationLevel_OutLift = false;
        public bool CarOnInLift = false, CarOnOutLift = false;
        public bool CarAssignedToOutputLift = false;
        [SerializeField] private float LiftIN_baseYval = 0, LiftOUT_baseYval = 0;
        [SerializeField] public Transform WaitPoint;
        public CarMovement CurrentCarOnLift;
        public CarMovement currentCarToDropOff = null;

        private void Start()
        {
            LiftIN_baseYval = IN_LiftPad.transform.position.y;
            LiftOUT_baseYval = OUT_LiftPad.transform.position.y;
        }

        private void AddToInputLiftQueue(CarMovement car)
        {
            InputCarsQueue.Add(car);
        }

        public void RemoveSpecificItemFromList(CarMovement car)
        {
            InputCarsQueue.Remove(car);
        }

        public void InputLiftQueueHandling(CarMovement car = null)
        {
            if(car != null)
                AddToInputLiftQueue(car);
            
            if (!CarOnInLift && CurrentCarOnLift == null)
            {
                //Assign the car (on the top of the queue) a new destination point of lift 
                CurrentCarOnLift = InputCarsQueue[0];
                CurrentCarOnLift.SetNewDestination(IN_LiftPad.transform);
            }
            else if(car != null)
            {
                car.SetNewDestination(WaitPoint);
                car.nextCarTransform = InputCarsQueue[^2].transform;
            }
        }
        
        public void AddLevelStopPoint(int levelID, GameObject levelPoint)
        {
            if (LevelStopPoints.Count < levelID)
            {
                LevelStopPoints.Add(levelPoint);
            }
        }
        
        public void LiftMovement(int floorID,LiftType lType)
        {
            if (LevelStopPoints.Count >= floorID)
            {
                if (lType == LiftType.IN)
                {
                   Move_InLift_Up(floorID);
                }
                else
                {
                    Move_OutLift(floorID);
                }
            }
        }

        public void OutputLiftHandling()
        {
            //If there are cars in the Queue, and no car has been assigned to the output lift, then get the 0th car
            if (!CarAssignedToOutputLift && currentCarToDropOff == null && parking.ReturnFloorBasedOutputQueueCount() != 0)
            {
                CarAssignedToOutputLift = true;
                currentCarToDropOff = parking.ReturnSpecificFloorBasedOutputQueueElement(0);
                LiftMovement(currentCarToDropOff.AssignedParkPoint.FloorID,LiftType.OUT);
            }
        }


        void Move_OutLift(int floorID)
        {
            Vector3 levelVector = new Vector3(OUT_LiftPad.transform.position.x,
                LevelStopPoints[floorID - 1].transform.position.y, OUT_LiftPad.transform.position.z);
                    
            //Taking Input Vector and moving up!
            OUT_LiftPad.transform.DOMove(levelVector, floorID*5).OnComplete(() =>
            {
                OUT_LiftPad_Collider.enabled = true;
                ReachedDestinationLevel_OutLift = true;
                //Inform the car to come to the lift
                currentCarToDropOff.SetNewDestination(OUT_LiftPad.transform);
            });
        }

        public void Move_OutLift_ToBaseLevel()
        {
            Vector3 levelVector = new Vector3(OUT_LiftPad.transform.position.x,
                LiftOUT_baseYval, OUT_LiftPad.transform.position.z);
            
            OUT_LiftPad.transform.DOMove(levelVector,currentCarToDropOff.AssignedParkPoint.FloorID * 5).OnComplete(()=>
            {
                OUT_LiftPad_Collider.enabled = false;
                ReachedDestinationLevel_OutLift = false;
                CarOnOutLift = false;
                currentCarToDropOff.CarReachedDropOffArea();
                currentCarToDropOff = null;
                CarAssignedToOutputLift = false;
                OutputLiftHandling();
            });
            parking.AddToAvailablePointsQueue(currentCarToDropOff.AssignedParkPoint);
            currentCarToDropOff.AssignedParkPoint.Car = null;
            currentCarToDropOff.AssignedParkPoint.isSpaceAvailable = true;
            currentCarToDropOff.AssignedParkPoint = null;
        }
        
        
        void Move_InLift_Up(int floorID)
        {
            Vector3 levelVector = new Vector3(IN_LiftPad.transform.localPosition.x,
                LevelStopPoints[floorID - 1].transform.localPosition.y, IN_LiftPad.transform.localPosition.z);
                    
            //Taking Input Vector and moving up!
            IN_LiftPad.transform.DOMove(levelVector, 5).OnComplete(() =>
            {
                IN_LiftPad_Collider.enabled = false;
                ReachedDestinationLevel_InLift = true;
                CurrentCarOnLift.MoveToPointOnFloor();
            });
        }

        public void Move_InLift_Down()
        {
            Vector3 levelVector = new Vector3(IN_LiftPad.transform.localPosition.x,
                LiftIN_baseYval, IN_LiftPad.transform.localPosition.z);
                    
            //Taking Input Vector and moving up!
            IN_LiftPad.transform.DOMove(levelVector, 5).OnComplete(() =>
            {
                IN_LiftPad_Collider.enabled = true;
                ReachedDestinationLevel_InLift = false;
                CarOnInLift = false;
                CurrentCarOnLift = null;
                InputLiftQueueHandling();
            });
        }
        
        
    }
}

public enum LiftType
{
    IN, OUT
}