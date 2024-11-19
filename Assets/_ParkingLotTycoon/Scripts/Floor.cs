using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    public class Floor: MonoBehaviour
    {
        public FloorTypes ThisFloor;

        public List<CarParkPoint> pointsForThisFloor;

        public void AddParkingPoints()
        {
            
        }
        
        public void SetFloorActive()
        {
            gameObject.SetActive(true);
        }

        public void ClearFloors()
        {
            gameObject.SetActive(false);
        }
    }
}