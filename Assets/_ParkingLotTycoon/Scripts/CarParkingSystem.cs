using System;
using System.Collections;
using System.Collections.Generic;
using ArcadeBridge;
using ArcadeBridge.ArcadeIdleEngine.Processors.Transformers;
using UnityEngine;
using UnityEngine.Serialization;

namespace ArcadeBridge
{
    //Car Parking Spots handling
    public class CarParkingSystem : MonoBehaviour
    {
        public int parkingSystemId = 0;
        [SerializeField] private List<CarParkPoint> parkPoints;
        [SerializeField] private List<CarParkPoint> availablePoints = new List<CarParkPoint>();
        [SerializeField] public List<CarMovement> carsInParkingLot;
        [SerializeField] private List<CarMovement> floorBasedOutputQueue;
        [SerializeField] private List<CarMovement> carsToLeaveParkingLotQueue;
        
        [SerializeField] private Transform stopPoint;
        [SerializeField] public Transform parkingSystemOutputPoint;
        
        [SerializeField] public GameObject parentObject;
        
        [SerializeField] public TicketingSystem ticketingSystem;
        [SerializeField] private TicketToCashPickableTransformStockpiler machine;
        [SerializeField] private FloorController floorController;
        
        public int NoOfUpgrades = 0;
        
        public LiftController lift;

        public Action<CarMovement> AddToOutputQueue, RemoveFromOutputQueue;
        
        private void OnEnable()
        {
            //Add all points to Queue in the start
            for(int i = 0; i < parkPoints.Count; i++) availablePoints.Add(parkPoints[i]);
            
            floorBasedOutputQueue = new List<CarMovement>();
            AddToOutputQueue += AddToFloorBasedOutputQueue;
            RemoveFromOutputQueue += RemoveFromFloorBasedOutputQueue;
            ParkingLotsHandler.addToParkingLotList?.Invoke(this);
            CarSpawningSystem.addToStopPointsList?.Invoke(stopPoint);
        }
        private void OnDisable()
        {
            AddToOutputQueue -= AddToFloorBasedOutputQueue;
            RemoveFromOutputQueue -= RemoveFromFloorBasedOutputQueue;
        }
        public void AddToCarsToLeaveParkingLotQueue(CarMovement car)
        {
            if(carsToLeaveParkingLotQueue.Count != 0)
                car.nextCarTransform = carsToLeaveParkingLotQueue[^1].transform;

            RemoveCarsFromFloorList(car);
            carsToLeaveParkingLotQueue.Add(car);
        }
        
        private void RemoveCarsFromFloorList(CarMovement car)
        {
            carsInParkingLot.Remove(car);
        }

        public CarParkPoint FindSpecificParkPoint(int FloorID, int PointID)
        {
            return parkPoints.Find(match: x => (x.FloorID == FloorID && x.pointID == PointID));
        }
        
        void RemoveFromFloorBasedOutputQueue(CarMovement car)
        {
            floorBasedOutputQueue.Remove(car);
        }
        void AddToFloorBasedOutputQueue(CarMovement car)
        {
            floorBasedOutputQueue.Add(car);
        }
        public CarMovement ReturnSpecificFloorBasedOutputQueueElement(int ind)
        {
            return floorBasedOutputQueue[ind];
        }
        public int ReturnFloorBasedOutputQueueCount()
        {
            return floorBasedOutputQueue.Count;
        }

        public void AddToAvailablePointsQueue(CarParkPoint point)
        {
            availablePoints.Add(point);
        }
        
        public CarParkPoint FindAvailableParkPoint()
        {
            if (availablePoints.Count != 0)
            {
                CarParkPoint currentPoint = availablePoints[0];
                availablePoints.RemoveAt(0);
                return currentPoint;
            }
            return null;
        }
        public void AddPointToCarParkingSystem(List<CarParkPoint> newPoints)
        {
            if(!lift.gameObject.activeInHierarchy)
                lift.gameObject.SetActive(true);

            foreach (CarParkPoint point in newPoints)
            {
                parkPoints.Add(point);
            }
        }
        
        public void UnlockNewFloor()
        {
            floorController.HandleFloors(NoOfUpgrades);
            NoOfUpgrades++;
        }

        public ParkingLots GetData()
        {
            //TODO: 
            ParkingLots thisData = new ParkingLots
            {
                availableTickets = ticketingSystem.tickets,
                outgoingCash = machine.ReturnAmountOfCash(),
                parkingLotID = parkingSystemId,
                noOfUpgrades = NoOfUpgrades,
                status = 1,
                totalCarsInParkingLot = carsInParkingLot.Count,
                //Set Cars List Data
                carsInParkingLot = GetCarsData()
            };

            return thisData;
        }

        List<Cars> GetCarsData()
        {
            List<Cars> allCarsData = new List<Cars>();
            Cars car = new Cars();

            for (int i = 0 ; i < carsInParkingLot.Count; i++)
            {
                car = carsInParkingLot[i].GetCarData();
            }
            allCarsData.Add(car);
            return allCarsData;
        }
        
        private ParkingLots SaveData;
        public void SetData(ParkingLots thisLotData)
        {
            SaveData = thisLotData;
            parkingSystemId = thisLotData.parkingLotID;
            ticketingSystem.tickets = thisLotData.availableTickets;
            machine.SpawnCash(thisLotData.outgoingCash);
            //Handle Parking Lot Upgrades before spawning cars
            for (int i = 0 ; i < thisLotData.noOfUpgrades; i++)
            {
                UnlockNewFloor();
            }
        }

        public void FindAndActivateSpecificCarsFromList(int floorID)
        {
            List<Cars> allCarsForSpecificFloor = SaveData.carsInParkingLot.FindAll(match: x => x.assignedParkPoint.floorID == floorID);
            SpawnAmountOfCars(allCarsForSpecificFloor);
        }
        
        public void SpawnAmountOfCars(List<Cars> allCarData)
        {
            for (int i = 0; i < allCarData.Count; i++)
            {
                CarMovement carSpawned = CarSpawningSystem.instantiateACar?.Invoke().GetComponent<CarMovement>();
                if (carSpawned != null) carSpawned.SetCarData(allCarData[i]);
            }
        }
        
        
        
        //Dependency Injection
        TicketingSystem _ticketingSystem;
        public void Inject(TicketingSystem ticketingSystem)
        {
            _ticketingSystem = ticketingSystem;
        }
        public void RemoveFromCarsToLeaveParkingLotQueue(CarMovement car)
        {
            carsToLeaveParkingLotQueue.Remove(car);
        }
        public CarMovement ReturnSpecificCarsToLeaveParkingLotQueueElement(int ind)
        {
            return carsToLeaveParkingLotQueue[ind];
        }
        public int ReturnCarsToLeaveParkingLotQueue()
        {
            return carsToLeaveParkingLotQueue.Count;
        }
    }
}