using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    public class ParkingLotsHandler : MonoBehaviour
    {
        [SerializeField] private List<CarParkingSystem> AllParkingLots;
        [SerializeField] private CarSpawningSystem prefabCarSpawningSystem;
        [SerializeField] public CarSpawningSystem currentCarSpawningSystem;
        
        public static Action<CarParkingSystem> addToParkingLotList;
        public static Func<GameObject,bool> AssignAParkingLane;
        public static Func<int, CarParkingSystem> parkingSystem;
        public static Func<CarSpawningSystem> GetSpawningSystemReference;
        
        private void Awake()
        {
            AllParkingLots = new List<CarParkingSystem>();
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            addToParkingLotList += AddToParkingLots;
            AssignAParkingLane += AddToCarParkingSystem;
            parkingSystem += ReturnParkingSystemInstance;
            GetSpawningSystemReference += ReturnSpawningSystemReference;
        }
        
        void AddToParkingLots(CarParkingSystem carParkingSystem)
        {
            AllParkingLots.Add(carParkingSystem);
            carParkingSystem.parkingSystemId = AllParkingLots.Count;
            if (currentCarSpawningSystem == null)
            {
                currentCarSpawningSystem = Instantiate(prefabCarSpawningSystem);
            }
        }

        CarSpawningSystem ReturnSpawningSystemReference()
        {
            return currentCarSpawningSystem;
        }

        CarParkingSystem ReturnParkingSystemInstance(int ind)
        {
            //Debug.Log("Index: " + ind + " list size: " + AllParkingLots.Count);
            return AllParkingLots[ind];
        }
        
        bool AddToCarParkingSystem(GameObject car)
        {
            CarMovement thisCar = car.GetComponent<CarMovement>();
            
            CarParkPoint parkPoint = AllParkingLots[thisCar.stopPointInd].FindAvailableParkPoint();
            
            //If we have no parking spots available we will get null
            if (parkPoint == null)
            {
                return false;
            }
            parkPoint.Car = car;
            return true;
        }
        
        private void OnDisable()
        {
            addToParkingLotList -= AddToParkingLots;
            AssignAParkingLane -= AddToCarParkingSystem;
            parkingSystem -= ReturnParkingSystemInstance;
            GetSpawningSystemReference -= ReturnSpawningSystemReference;
        }
    }
}
