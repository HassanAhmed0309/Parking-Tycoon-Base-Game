using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ArcadeBridge
{
    public class LevelHandler : MonoBehaviour
    {
        [SerializeField] private List<CarParkingSystem> allCarParkingSystems;
        [SerializeField] private List<TicketingSystem> allTicketSystems;
        [SerializeField] private List<AI> allUnlockedAI;
        public void SetLevelData(LevelDataHolder levelData)
        {
            //Handle Character data
            GameManager.Instance.SetCharacterData(levelData.characterData);
            
            
            
            //Handling Parking Lots Data
            for (int i= 0; i < levelData.parkingSystems.Count; i++)
            {
                if (levelData.parkingSystems[i].status == 1)
                {
                    //Activate the object
                    allCarParkingSystems[i].parentObject.SetActive(true);
                    
                    //And Set the data
                    allCarParkingSystems[i].SetData(levelData.parkingSystems[i]);
                }
            }
            
            
            //TODO: Handle AI Data
        }

        public LevelDataHolder GetLevelData()
        {
            LevelDataHolder allLevelData = new LevelDataHolder();
            allLevelData.parkingSystems = new List<ParkingLots>();
            for (int i = 0; i < allCarParkingSystems.Count; i++)
            {
                ParkingLots lot = new ParkingLots();
                if (allCarParkingSystems[i].gameObject.activeInHierarchy)
                {
                    //Populate the lot object
                    lot = allCarParkingSystems[i].GetData();
                }
                else
                {
                    lot.status = 0;
                }
                allLevelData.parkingSystems.Add(lot);
            }

            Character character = GameManager.Instance.GetCharacterData();

            allLevelData.characterData = character;
            
            return allLevelData;
        }
    }
}
