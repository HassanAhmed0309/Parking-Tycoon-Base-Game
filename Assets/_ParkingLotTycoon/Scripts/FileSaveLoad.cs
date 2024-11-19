using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ArcadeBridge
{
    public class FileSaveLoad
    {
        public bool LoadData<T>(string Filepath, out T data)
        {
            string path = Application.persistentDataPath + Filepath;
            T GameData = default;
            if (File.Exists(path))
            {
                Debug.Log("File " + Filepath + " exists!");
                var jsonData = File.ReadAllText(path);
                data = JsonConvert.DeserializeObject<T>(jsonData);
                return true;
            }
            data = GameData;
            return false;
        }
        
        public void SaveData<T>(string Filepath, T data)
        {
            string path = Application.persistentDataPath + Filepath;
            Debug.Log("Data: " + data);
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            Debug.Log("Json Data: " + jsonData);
            File.WriteAllText(path, jsonData);
        }
    }
}
