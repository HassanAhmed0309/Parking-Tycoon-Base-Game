using System;
using System.Collections.Generic;
using System.IO;
using ArcadeBridge.ArcadeIdleEngine.OdinSerializer;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Data
{
	[CreateAssetMenu(menuName = nameof(ArcadeIdleEngine) + "/" + nameof(Data) + "/" + nameof(SaveManager))]
	public class SaveManager : ScriptableObject
	{
		const int SAVE_VERSION = 0;
		const DataFormat DATA_FORMAT = DataFormat.JSON;
		
		[SerializeField] List<Saveable> _saveables;
		
		SaveData _saveData = new SaveData();
		SaveUpgrader _saveUpgrader = new SaveUpgrader();
		string _savePath;

		public event Action RestoreCompleted;

		void OnEnable()
        {
			_savePath = Path.Combine(Application.persistentDataPath, "gamedata.json");
        }

        public void RestoreAll()
		{
			if (File.Exists(_savePath))
			{
				byte[] bytes = File.ReadAllBytes(_savePath);
				SaveData saveData = SerializationUtility.DeserializeValue<SaveData>(bytes, DATA_FORMAT);

				foreach (Saveable saveableSO in _saveables)
				{
					if (saveData.Saves.TryGetValue(saveableSO.GetGuid, out object save))
					{
                        saveableSO.RestoreState(save);
                    }
                }

				_saveUpgrader.CheckAndUpgrade(saveData, SAVE_VERSION);
			}
			else
			{
				foreach (Saveable saveableSO in _saveables)
				{
					saveableSO.RestoreState(saveableSO.GetDefaultValue);
				}
			}
			
			RestoreCompleted?.Invoke();
		}

		public void SaveAll()
		{
			foreach (Saveable saveableSO in _saveables)
			{
				if (_saveData.Saves.ContainsKey(saveableSO.GetGuid))
				{
					_saveData.Saves[saveableSO.GetGuid] = saveableSO.CaptureState();
				}
				else
				{
					_saveData.Saves.Add(saveableSO.GetGuid, saveableSO.CaptureState());
				}
			}
			_saveData.Version = SAVE_VERSION;
			byte[] convertedSaveData = SerializationUtility.SerializeValue(_saveData, DATA_FORMAT);
			File.WriteAllBytes(_savePath, convertedSaveData);
		}
	}
}