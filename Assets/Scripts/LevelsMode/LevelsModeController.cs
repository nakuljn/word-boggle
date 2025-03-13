using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WordBoggle
{
    /// <summary>
    /// This class controls the level mode gameplay, It loads the level data and initialises the grid
    /// initialises the level type based on the level data. Tells the grid manager about game end actions.
    /// </summary>
    public class LevelsModeController : MonoBehaviour
    {
        #region Fields
        
        [SerializeField] private LevelManager levelManager;
        
        [Header("Top Panel")]
        [SerializeField] private GameObject timerObj;
        [SerializeField] private TextMeshProUGUI timerTxt;
        [SerializeField] private TextMeshProUGUI currentTarget;
        [SerializeField] private TextMeshProUGUI totalTarget;
        
        [Header("Botom Panel")]
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button prevLevelButton;
        [SerializeField] private Button unlockNextLevelButton;
        [SerializeField] private TextMeshProUGUI levelTxt;
        
        private LevelType _levelType;
        private int _currentScore;
        private int _currentWordCount = 0;
        private int _wordsTargetCount = 1;
        private Action<bool> _onGameEnd;
        private bool _unlockButtonClicked = false;

        #endregion
        #region Unity Methods
        private void OnEnable()
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(LoadNextLevel);
            
            prevLevelButton.onClick.RemoveAllListeners();
            prevLevelButton.onClick.AddListener(LoadPreviousLevel);
            
            unlockNextLevelButton.onClick.RemoveAllListeners();
            unlockNextLevelButton.onClick.AddListener(UnlockNextLevel);
            EnableUIElements();
        }

        //For testing next level
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                _unlockButtonClicked = true;
                LoadNextLevel();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                LoadPreviousLevel();
            }
        }
        
        private void OnDisable()
        {
            unlockNextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.RemoveAllListeners();
            prevLevelButton.onClick.RemoveAllListeners();
            DisableUIElements();
        }
        
        #endregion
        
        
        #region Levels Setup
        public void StartGame(Grid grid, int currentLevel, Action<bool> onGameEnd)
        {
            _unlockButtonClicked = false;
            unlockNextLevelButton.interactable = true;
            nextLevelButton.interactable = false;
            _levelType = LevelType.NoTimeLimit;
            _onGameEnd = onGameEnd;
            levelTxt.text = (currentLevel + 1).ToString();
            
            var levelData = levelManager.LoadLevel(currentLevel);

            SetLevelType(levelData);
            
            _wordsTargetCount = levelData.wordCount;
            timerObj.gameObject.SetActive(_levelType != LevelType.NoTimeLimit);

            grid.Initialise(levelData);
            SetLevelTypeUI(levelData.wordCount, levelData.totalScore);
            
            if (_levelType == LevelType.TimeLimitForWords)
            {
                StartCoroutine(StartTimer(levelData.timeSec, levelData.totalScore));
            }
        }

        private void SetLevelType(LevelData levelData)
        {
            if (levelData.timeSec > 0 && levelData.totalScore == 0)
            {
                _levelType = LevelType.TimeLimitForWords;
            }
            else if (levelData.timeSec > 0 && levelData.totalScore > 0)
            {
                _levelType = LevelType.TimeLimitForScore;
            }
            else
            {
                _levelType = LevelType.NoTimeLimit;
            }
        }

        private void SetLevelTypeUI(int wordCount, int totalScore)
        {
            if (_levelType == LevelType.NoTimeLimit || _levelType == LevelType.TimeLimitForWords)
            {
                currentTarget.text = UIStrings.CurrentWords + _currentWordCount;
                totalTarget.text =   UIStrings.WordsTarget + wordCount;
            }

            if (_levelType == LevelType.TimeLimitForScore)
            {
                currentTarget.text = UIStrings.CurrentScore + _currentScore;
                totalTarget.text = UIStrings.ScoreTarget + totalScore;
            }
        }
        
        #endregion

        
        #region Gameplay Functions
        private IEnumerator StartTimer(int totalTime, int scoreToReach)
        {
            float elapsedTime = totalTime;
            while (elapsedTime > 0)
            {
                elapsedTime -= Time.deltaTime;
                timerTxt.text = elapsedTime.ToString("0.00");
                yield return null;
            }

            if (_levelType == LevelType.TimeLimitForWords &&_currentWordCount >= _wordsTargetCount)
                _onGameEnd?.Invoke(true);
            else
                _onGameEnd?.Invoke(false);

            if (_levelType == LevelType.TimeLimitForScore && _currentScore >= scoreToReach)
                _onGameEnd?.Invoke(true);
            else
                _onGameEnd?.Invoke(false);
        }

        public int GetTotalScore()
        {
            return _currentScore;
        }
        public void UpdateScore(int currentScore, int wordsFormed)
        {
            _currentScore += currentScore;
            totalTarget.text += _currentScore.ToString();
            _currentWordCount++;
            
            if (_levelType == LevelType.TimeLimitForWords || _levelType == LevelType.TimeLimitForScore)
            {
                currentTarget.text = UIStrings.CurrentWords + _currentWordCount;
            }

            if (_levelType == LevelType.TimeLimitForScore)
            {
                currentTarget.text = UIStrings.CurrentScore + _currentScore;
            }
            
            if (_levelType == LevelType.NoTimeLimit && _currentWordCount >= _wordsTargetCount)
            {
                _onGameEnd?.Invoke(true);
            }
        }

        public void EnableNextLevelButton()
        {
            _unlockButtonClicked = true;
            nextLevelButton.interactable = true;
        }

        private void LoadNextLevel()
        {
            if (_unlockButtonClicked)
            {
                var playerData = PlayerDataService.Instance.LoadPlayerData();
                int prevLevel = playerData.LevelsGameData.currentLevel + 1;
                PlayerDataService.Instance.SetPlayerLevel(prevLevel);
            }
            
            GameManager.Instance.StartLevelsGame();
        }
        
        private void LoadPreviousLevel()
        {
            var playerData = PlayerDataService.Instance.LoadPlayerData();
            int prevLevel = playerData.LevelsGameData.currentLevel - 1;
            
            if (prevLevel <= 0)
                return;
            
            PlayerDataService.Instance.SetPlayerLevel(prevLevel);
            GameManager.Instance.StartLevelsGame();
        }

        private void UnlockNextLevel()
        {
            nextLevelButton.interactable = true;
            unlockNextLevelButton.interactable = false;
        }
        private void EnableUIElements()
        {
            timerObj.gameObject.SetActive(true);
            currentTarget.gameObject.SetActive(true);
            totalTarget.gameObject.SetActive(true);
            nextLevelButton.gameObject.SetActive(true);
            prevLevelButton.gameObject.SetActive(true);
            levelTxt.gameObject.SetActive(true);
        }
        
        private void DisableUIElements()
        {
            timerObj.gameObject.SetActive(false);
            currentTarget.gameObject.SetActive(false);
            totalTarget.gameObject.SetActive(false);
            prevLevelButton.gameObject.SetActive(false);
            nextLevelButton.gameObject.SetActive(false);
            levelTxt.gameObject.SetActive(false);

        }
        
        #endregion
    }
}