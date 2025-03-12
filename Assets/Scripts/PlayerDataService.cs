using System;
using System.IO;
using UnityEngine;

namespace  WordBoggle
{
    public class PlayerDataService : MonoBehaviour
    {
        #region Singleton
        public static PlayerDataService Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        #endregion
        
        private string _filePath;
        private PlayerData _playerData;

        private void Start()
        {
            _filePath = Application.persistentDataPath + "/playerData.json";
        }
        public void CreatePlayerData(string playerName)
        {
            _playerData = new PlayerData
            {
                playerId = Guid.NewGuid().ToString(), 
                playerName = "Player", 
                endlessGameData = new PlayerEndlessGameData(),
                LevelsGameData = new PlayerLevelsGameData()
            };
            
            SavePlayerData(_playerData);
        }

        public PlayerData LoadPlayerData()
        {
            if (!File.Exists(_filePath))
            {
               CreatePlayerData("New User");
            }
            
            string json = File.ReadAllText(_filePath);
            _playerData = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Player Data Loaded: " + json);
            
            return _playerData;
        }

        public bool IsNewUser()
        {
            return !File.Exists(_filePath);
        }

        private void SavePlayerData(PlayerData playerData)
        {
            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(_filePath, json);
            Debug.Log("Player Data Saved: " + json);
        }
        
        public void IncreaseLevelByOne()
        {
            _playerData.LevelsGameData.currentLevel += 1;
            SavePlayerData(_playerData);
        }

        public void SetPlayerLevel(int level)
        {
            _playerData.LevelsGameData.currentLevel = level;
            SavePlayerData(_playerData);
        }
        public void UpdatePlayerScore(int playerScore)
        {
            _playerData.LevelsGameData.totalScore += playerScore;
            SavePlayerData(_playerData);
        }
    }

    [Serializable]
    public class PlayerData
    {
        public string playerId;
        public string playerName;
        public PlayerEndlessGameData endlessGameData;
        public PlayerLevelsGameData LevelsGameData;
    }

    [Serializable]
    public class PlayerEndlessGameData
    {
        public int currentLevel = 1;
        public int totalScore = 0;
    }

    [Serializable]
    public class PlayerLevelsGameData
    {
        public int currentLevel = 1;
        public int totalScore = 0;
    }

}
