using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    public class FloorController : MonoBehaviour
    {
        [SerializeField] private List<Floor> allFloorObjects;
        [SerializeField] private List<FloorTypeHandle> upgradesList;

        public void HandleFloors(int upgradeNo)
        {
            ActivateAllRequiredFloors(upgradeNo);
            DeactivateAllRequiredFloors(upgradeNo);
        }

        private void ActivateAllRequiredFloors(int upgradeNo)
        {
            List<FloorTypes> floors = upgradesList[upgradeNo].ActiveFloors;
            int activeFloorsCount = floors.Count;
            if (activeFloorsCount != 0)
            {
                Floor active = null;
                for (int i = 0; i < activeFloorsCount; i++)
                {
                    active = allFloorObjects.Find(match:x=> x.ThisFloor == floors[i]);
                    active.SetFloorActive();
                }
            }
        }
        private void DeactivateAllRequiredFloors(int upgradeNo)
        {
            List<FloorTypes> floors = upgradesList[upgradeNo].InActiveFloors;
            int inActiveFloorsCount = floors.Count;
            if (inActiveFloorsCount != 0)
            {
                Floor inActive = null;
                for (int i = 0; i < inActiveFloorsCount; i++)
                {
                    inActive = allFloorObjects.Find(match:x=> x.ThisFloor == floors[i]);
                    inActive.ClearFloors();
                }
            }
        }
    }
}

[Serializable]
class FloorTypeHandle
{
    public List<FloorTypes> ActiveFloors;
    public List<FloorTypes> InActiveFloors;
}

public enum FloorTypes
{
    MudFloor,
    GrassBase,
    GreyFloor,
    MultiFloorGreyFloor,
    Floors
}