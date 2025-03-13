using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WordBoggle
{
    
    public enum GameMode{
        Endless,
        Levels
    }
    public enum LevelType
    {
        NoTimeLimit,
        TimeLimitForWords,
        TimeLimitForScore
    }
    
    /// <summary>
    /// Central class for handling game flow. sets up the game mode,
    /// processes user input and handles game win/lose actions, 
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton

        public static GameManager Instance;

        private void Awake()
        {
            if (Instance == null || Instance != this)
            {
                Instance = this;
                return;
            }
            Destroy(gameObject);
        }

        #endregion

        #region Fields

        [SerializeField] private InputManager inputManager;
        
        [Header("Top Panel")]
        [SerializeField] private TextMeshProUGUI averageScore;
        [SerializeField] private TextMeshProUGUI totalScore;
        
        [Header("Grid Panel")]
        [SerializeField] private TextMeshProUGUI displayWord;
        [SerializeField] private TextMeshProUGUI errorState;
        [SerializeField] private Grid grid;
        [SerializeField] private GameObject gameWinObject;
        [SerializeField] private GameObject gameLoseObject;
        
        [Header("Bottom Panel")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button homeButton;
        
        [Header("Game Modes")]
        [SerializeField] private LevelsModeController levelsModeController;
        [SerializeField] private EndlessModeController endlessModeController;
        
        [SerializeField] private float animTime = 1f;
        
        public GameMode GameMode { get; private set; } = GameMode.Endless;
        
        private HashSet<string> _wordSet;

        private int _currentScore = 0;
        private int _currentWords = 0;
        
        
        private Coroutine _errorAnim;

       #endregion
        

        #region Unity Methods
        private void OnEnable()
        {
            inputManager.OnSelectionStart -= ClearDisplayWord;
            inputManager.OnSelectionStart += ClearDisplayWord;
            
            inputManager.OnSelectionComplete -= OnWordSelected;
            inputManager.OnSelectionComplete += OnWordSelected;
            
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartLevel);
            
            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(LoadHomeScene);
        }

        void Start()
        {
            _wordSet = new HashSet<string>();
            GameMode = GameMode.Endless;
            gameWinObject.SetActive(false);
            LoadAllWords();
            
            SetupGameData();
        }
        
        private void OnDisable()
        {
            inputManager.OnSelectionStart -= ClearDisplayWord;
            inputManager.OnSelectionComplete -= OnWordSelected;
            restartButton.onClick.RemoveAllListeners();
            homeButton.onClick.RemoveAllListeners();
            StopAllCoroutines();
        }
        
        #endregion
        
        private void SetupGameData()
        {
            if (PlayerPrefs.HasKey(Constants.PlayerPrefsConstants.GameMode))
            {
                var gameModeString = PlayerPrefs.GetString(Constants.PlayerPrefsConstants.GameMode);
                Enum.TryParse(gameModeString, out GameMode gameMode);
                GameMode = gameMode;
            }
            switch (GameMode)
            {
                case GameMode.Endless:
                    StartEndlessGame();
                    break;
                case GameMode.Levels:
                    StartLevelsGame();
                    break;
                default:
                    throw new Exception("Game mode not supported");
            }
        }
        
        private void LoadAllWords()
        {
            TextAsset textFile = Resources.Load<TextAsset>("wordlist");
            if (textFile != null)
            {
                _wordSet = new HashSet<string>(
                    textFile.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(word => word.Trim())
                );
            }
            else
            {
                Debug.LogError("File not found in Resources!");
            }
        }
        
        private void StartEndlessGame()
        {
            levelsModeController.gameObject.SetActive(false);
            endlessModeController.gameObject.SetActive(true);
            endlessModeController.StartEndlessGame(grid, _wordSet);
        }

        public void StartLevelsGame()
        {
            var playerData = PlayerDataService.Instance.LoadPlayerData();
            endlessModeController.gameObject.SetActive(false);
            levelsModeController.gameObject.SetActive(true);
            levelsModeController.StartGame(grid, playerData.LevelsGameData.currentLevel, GameEnded);
        }

        
        private void OnWordSelected(List<LetterTile> tilesMatched)
        {
            if (tilesMatched.Count == 0)
            {
                return;
            }
            if (tilesMatched.Count < 3)
            {
                ShowErrorMessage(UIStrings.WordTooShort);
                return;
            }
            
            string word = "";
            int score = 0;
            foreach (var letterTile in tilesMatched)
            {
                word += letterTile.GetLetterString();
                score += letterTile.GetLetterScore();
            }
            
            displayWord.text = word;
            
            word = word.ToLower();
            if (!_wordSet.Contains(word))
            {
                ShowErrorMessage(UIStrings.WordNotFound);
                return;
            }
            _currentScore += score;
            _currentWords++;
            
            DisplayScores();
            switch (GameMode)
            {
                case GameMode.Endless:
                    endlessModeController.RemoveMatchedTiles(tilesMatched);
                    break;
                case GameMode.Levels:
                    levelsModeController.UpdateScore(_currentScore, _currentWords);
                    break;
            }
        }

        private void DisplayScores()
        {
            var avgScore = _currentScore / _currentWords;

            totalScore.text = "Total Score : " + _currentScore.ToString();
            averageScore.text = "Average Score: " + avgScore.ToString();
        }

        private void ShowErrorMessage(string message)
        {
            if (_errorAnim != null)
            {
                StopCoroutine(_errorAnim);
                _errorAnim = null;
                errorState.text = "";
            }

            errorState.alpha = 1;
            errorState.text = message;
            _errorAnim = StartCoroutine(FadeOutErrorMessage());
        }

        private IEnumerator FadeOutErrorMessage()
        {
            yield return new WaitForSeconds(1f);
            var elapsedTime = 0f;

            while (elapsedTime < animTime)
            {
                elapsedTime += Time.deltaTime;
                errorState.alpha = 1 - (elapsedTime / animTime);
                yield return null;
            }
        }
        
        private void ClearDisplayWord()
        {
            displayWord.text = "";
        }
        
        private void GameEnded(bool isWin)
        {
            if (isWin)
            {
                StartCoroutine(GameWinRoutine());
            }
            else
            {
                gameLoseObject.SetActive(true);
            }
        }

        private IEnumerator GameWinRoutine()
        {
            yield return null;
            PlayerDataService.Instance.IncreaseLevelByOne();
            int endLevelScore = levelsModeController.GetTotalScore();
            PlayerDataService.Instance.UpdatePlayerScore(endLevelScore);
            
            gameWinObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            gameWinObject.SetActive(false);

            if (GameMode == GameMode.Levels)
            {   
                LoadNextLevel();
            }
        }
        
        private void LoadNextLevel()
        { 
            grid.RemoveCells();
            levelsModeController.EnableNextLevelButton();
        }

        private void LoadHomeScene()
        {
            SceneManager.LoadScene(Constants.SceneNames.HomeScene);
        }

        private void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void ToggleGridLayoutGroup(bool toggle)
        {
            grid.ToggleGridLayoutGroup(toggle);
        }

    }
}