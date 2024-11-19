using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace ArcadeBridge
{
    public class LevelManager : MonoBehaviour
    {
        //Ask SaveLoad System for Save File.
        //If save file exists, give the file to level prefab to load from
        //else Load default file

        [SerializeField] private List<LevelHandler> AllLevels; 
        
        [SerializeField] private GameObject SaveAndLoadPrefab;

        public GameStateSaveLoad SaveAndLoadInstance;

        public LevelHandler currentActiveLevel;
        
        public int currentLevelNo = 1;
        
        [Tooltip("Testing Bool, remove in final build")]
        [SerializeField] public bool spawnLevel = true;
        
        private void Awake()
        {
            SaveAndLoadInstance = Instantiate(SaveAndLoadPrefab).GetComponent<GameStateSaveLoad>();
            SaveAndLoadInstance.LevelManager = this;
            if(spawnLevel)
                SaveAndLoadInstance.CheckForFile();
        }
        public void SetupLevel(bool isLevelStateAvailable, GameState levelState)
        {
            LevelDataHolder currentLevelData = new LevelDataHolder();
            if(isLevelStateAvailable)
            {
                currentLevelNo = levelState.CurrentLevelNo;                
                //Create File Name for level
                string levelDataPath = "/Level" + levelState.CurrentLevelNo + ".json";
                
                //Check for the level Data file
                SaveAndLoadInstance.fileReadWrite.LoadData(levelDataPath, out LevelDataHolder levelData);

                currentLevelData = levelData;
            }
            else
            {
                //Create a GameState Object with default data
                GameState state = new GameState();
                state.CurrentLevelNo = currentLevelNo;
                
                //Create a file that stores current Level Data
                SaveAndLoadInstance.fileReadWrite.SaveData("/GameState.json",state);
                
                LevelDataHolder data1 = LevelDeserializedData();
                currentLevelData = data1;
            }

            currentActiveLevel = Instantiate(AllLevels[0].gameObject).GetComponent<LevelHandler>();
            if (currentLevelData != null)
            {
                //Set Level data File
                AllLevels[currentLevelNo-1].SetLevelData(currentLevelData);
            }
            else
            {
                Debug.Log("Current Level Data is null!");
            }
        }

        private LevelDataHolder LevelDeserializedData()
        {
            string levelDataPath = "Level1";
            //Load meta file from resources folder
            TextAsset obj = Resources.Load<TextAsset>(levelDataPath);
            LevelDataHolder levelData = JsonConvert.DeserializeObject<LevelDataHolder>(obj.text);
            return levelData;
        }
    }
}
 
