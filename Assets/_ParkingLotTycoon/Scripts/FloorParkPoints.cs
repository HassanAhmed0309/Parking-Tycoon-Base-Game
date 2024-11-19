using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    public class FloorParkPoints : MonoBehaviour
    {
        public int FloorID;
        [SerializeField] private List<CarParkPoint> FloorPoints;
        [SerializeField] private CarParkingSystem ParkingSystem;

        private void OnEnable()
        {
            for (int i= 0; i < FloorPoints.Count; i++)
            {
                FloorPoints[i].FloorID = FloorID;
            }
            //Add these points to the Car Parking System
            ParkingSystem.AddPointToCarParkingSystem(FloorPoints);
            ParkingSystem.lift.AddLevelStopPoint(FloorID,gameObject);
            
            //Get specific cars from the load file and spawn them here
            ParkingSystem.FindAndActivateSpecificCarsFromList(FloorID);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CarMovement carMovement))
            {
                ParkingSystem.lift.Move_InLift_Down();
            }
        }
    }
}
