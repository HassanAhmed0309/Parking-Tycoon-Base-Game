using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = System.Random;

namespace ArcadeBridge
{
    public class CarParkPoint : MonoBehaviour
    {
        private Transform point;
        private GameObject car = null;

        public Transform waypoint
        {
            get
            {
                return point;
            }
            set
            {
                point = this.transform;
            }
        }
        public GameObject Car
        {
            get => car;
            set
            {
                car = value;
                if (car == null)
                {
                    isSpaceAvailable = true;
                    
                    OnSpaceAvailable?.Invoke();
                }
                else 
                {
                    isSpaceAvailable = false;
                    if (waypoint == null)
                        waypoint = null;
                    CarMovement carMovement = Car.GetComponent<CarMovement>();
                    carMovement.AssignedParkPoint = this;
                    carMovement.timer = UnityEngine.Random.Range(5, 12);
                }
            }
        }

        public static Action OnSpaceAvailable;
        public bool isSpaceAvailable = true;
        public int FloorID = 0;
        public int pointID = 0;

        // private void Start()
        // {
        //     car = null;
        // }

        public void SetParkPointData(ParkPoint parkPointData, CarMovement carObject)
        {
            isSpaceAvailable = parkPointData.availibility;
            car = carObject.gameObject;
            FloorID = parkPointData.floorID;
        }

        public ParkPoint GetParkPointData()
        {
            ParkPoint point = new ParkPoint();

            point.availibility = isSpaceAvailable;

            point.parkingPointNo = pointID;

            point.floorID = FloorID;
            
            return point;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CarMovement carMove))
            {
                if(carMove.AssignedParkPoint == null)
                    carMove.AssignedParkPoint = this;
                carMove.reachedParkingSpot = true;
                //Car on base floor
                if (FloorID < 1)
                {
                    //Debug.Log("Car Reached Parking Spot!");
                    carMove.DespawnTime();
                }
                //Car on Higher floors
                else
                {
                    carMove.WaitTimeCompleted();
                }
            }
        }
    }
}
