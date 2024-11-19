using System;
using System.Collections;
using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Actors;
using ArcadeBridge.ArcadeIdleEngine.Data;
using ArcadeBridge.ArcadeIdleEngine.Inventory;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using Cinemachine;
using Newtonsoft.Json;
using UnityEngine;

namespace ArcadeBridge
{
    public class GameManager : MonoBehaviour
    {
        [Header("Prefabs To Spawn")] 
        [SerializeField] private GameObject LevelManagerPrefab;

        [SerializeField] private IntVariable money = null;
        [SerializeField] private IntVariable gems = null;

        [SerializeField] private GameObject playerPrefab;

        [SerializeField] private ArcadeIdleMover playerSpeed;
        [SerializeField] private InventoryManager playerCapacity;

        [SerializeField] private CinemachineVirtualCamera vCam;
        
        [Header("Public References")]
        [SerializeField] public LevelManager currentLevelManager;
        
        public static GameManager Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
        }

        public void SetVcamFollow(GameObject target)
        {
            vCam.Follow = target.transform;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            //Spawn the Level Manager
            currentLevelManager = Instantiate(LevelManagerPrefab).GetComponent<LevelManager>();
            if (!currentLevelManager.spawnLevel)
            {
                GameObject player = Instantiate(playerPrefab);
                player.SetActive(true);
                playerSpeed = player.GetComponent<ArcadeIdleMover>();
                playerCapacity = player.GetComponent<InventoryManager>();
                SetVcamFollow(player);
            }
        }

        public void SetCharacterData(Character charData)
        {
            GameObject player = Instantiate(playerPrefab, charData.transform.pos,
                Quaternion.Euler(new Vector3(charData.transform.rot.x,charData.transform.rot.y,charData.transform.rot.z)));
            playerSpeed = player.GetComponent<ArcadeIdleMover>();
            playerCapacity = player.GetComponent<InventoryManager>();
            player.SetActive(true);
            SetVcamFollow(player);

            money.RuntimeValue = charData.money;
            //gems.RuntimeValue = charData.gems;

            playerSpeed.SetCharacterSpeedData(charData.speed);
            playerCapacity.SetCharacterCapacity(charData.capacity);
        }

        private GameObject Instantiate(GameObject original, CustomVector3 transformPos, Quaternion instantiateInWorldSpace)
        {
            return GameObject.Instantiate(original, new Vector3(transformPos.x, transformPos.y, transformPos.z),
                instantiateInWorldSpace);
        }

        public Character GetCharacterData()
        {
            Character character = new Character();

            var transform1 = playerSpeed.transform;
            character.transform.SetPosVector(transform1.position);
            character.transform.SetRotVector(transform1.rotation.eulerAngles);
            character.transform.SetScaleVector(transform1.localScale);

            character.capacity = playerCapacity.GetCharacterCapacity();

            character.speed = playerSpeed.GetCharacterSpeed();

            character.money = money.RuntimeValue;
            //character.gems = gems.RuntimeValue;
            
            return character;
        }

        [ContextMenu("Save Data")]
        void SaveAllData()
        {
            LevelDataHolder data = currentLevelManager.currentActiveLevel.GetLevelData();
            currentLevelManager.SaveAndLoadInstance.fileReadWrite.SaveData("/Level1.json", data);
        }

        [ContextMenu("Set Cash to 1000")]
        void SetCash()
        {
            money.RuntimeValue = 1000;
        }
    }
}
