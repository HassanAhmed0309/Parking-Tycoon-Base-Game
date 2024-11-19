using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ArcadeBridge
{
    public class GameState
    {
        public int CurrentLevelNo = 1;
    }
}

[Serializable]
public class LevelDataHolder
{
    public List<ParkingLots> parkingSystems=new List<ParkingLots>();
    //public List<TicketSystem> ticketingSystems;
    public Character characterData=new Character();
    //public AI aIData;
}

[Serializable]
public struct ParkingLots
{
    public int parkingLotID;
    public int availableTickets;
    public int outgoingCash;
    public int totalCarsInParkingLot;
    public List<Cars> carsInParkingLot;
    public int noOfUpgrades;
    public int status;
}
[Serializable]
public struct Cars
{
    public int timeAssigned;
    public GTransform carPosition;
    public int floorID;
    public ParkPoint assignedParkPoint;
    public float initialSpeed;
    public GTransform destination;
}
[Serializable]
public struct ParkPoint
{
    public bool availibility;
    public int floorID;
    public int parkingPointNo;
}
[Serializable]
public struct TicketSystem
{
    public int ticketID;
    public int currentAmountOfTickets;
}
[Serializable]
public struct Character
{
    public GTransform transform;
    public int money;
    public int gems;
    public CharacterSpeed speed;
    public CharacterCapacity capacity;
}
[Serializable]
public struct CharacterSpeed
{
    public float currentSpeed;
    public int tempSpeedMultiplier;
    public int noOfUpgrades;
}

[Serializable]
public struct CharacterCapacity
{
    public int currentCapacity;
    public int tempCapacityAdder;
    public int noOfUpgrades;
}
[Serializable]
public struct GTransform
{
    public CustomVector3 pos;
    public CustomVector3 rot;
    public CustomVector3 scale;
    
    public void SetPosVector(Vector3 vector)
    {
        pos.x = vector.x;
        pos.y = vector.y;
        pos.z = vector.z;
    }
    public void SetRotVector(Vector3 vector)
    {
        rot.x = vector.x;
        rot.y = vector.y;
        rot.z = vector.z;
    }
    public void SetScaleVector(Vector3 vector)
    {
        scale.x = vector.x;
        scale.y = vector.y;
        scale.z = vector.z;
    }
}

[Serializable]
public struct CustomVector3
{
    public float x;
    public float y;
    public float z;

}
[Serializable]
public struct AI
{
    public int amountOfUnlockedAI;
    public int speedOfAI;
    public int capacityOfAI;
    public int noOfSpeedUpgrades;
    public int noOfCapacityUpgrades;
}