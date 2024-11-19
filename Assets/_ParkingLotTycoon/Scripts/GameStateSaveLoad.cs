using Newtonsoft.Json;
using UnityEngine;

namespace ArcadeBridge
{
    public class GameStateSaveLoad : MonoBehaviour
    {
        public GameState LastGameState = null;
        public FileSaveLoad fileReadWrite = new FileSaveLoad();
        public LevelManager LevelManager;
        public void CheckForFile()
        {
            bool fileExists = fileReadWrite.LoadData("/GameState.json", out GameState gameState);
            Debug.Log("file Exists: " + fileExists + " Game State is null: " + (gameState == null));
            LevelManager.SetupLevel(fileExists,gameState);
        }
    }
}
