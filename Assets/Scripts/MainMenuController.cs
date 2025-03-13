using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace WordBoggle
{
    /// <summary>
    /// UI Viewer class responsible for the home scene interaction
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        #region Fields

        [Header("Input Game Fields")] 
        [SerializeField] private TMP_InputField playerNameInputField;

        [SerializeField] private Toggle endlessModeToggle;
        [SerializeField] private Toggle levelsModeToggle;

        [SerializeField] private Button startGameButton;
        [SerializeField] private Button quitGameButton;

        private GameMode _gameMode = GameMode.Levels;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            RemoveAllListeners();
            AddListeners();
        }
        private void Start()
        {
            List<string> levelTypes = Enum.GetNames(typeof(LevelType)).ToList();
        }

        private void OnDisable()
        {
            RemoveAllListeners();
        }

        #endregion

        private void AddListeners()
        {
            playerNameInputField.onValueChanged.AddListener(OnPlayerNameChanged);
            endlessModeToggle.onValueChanged.AddListener(SelectEndlessMode);
            levelsModeToggle.onValueChanged.AddListener(SelectLevelsMode);
            startGameButton.onClick.AddListener(StartGame);
            quitGameButton.onClick.AddListener(QuitGame);
        }

        private void RemoveAllListeners()
        {
            playerNameInputField.onValueChanged.RemoveAllListeners();
            endlessModeToggle.onValueChanged.RemoveAllListeners();
            levelsModeToggle.onValueChanged.RemoveAllListeners();
            startGameButton.onClick.RemoveAllListeners();
            quitGameButton.onClick.RemoveAllListeners();
        }

        private void OnPlayerNameChanged(string playerName)
        {
            if (String.IsNullOrEmpty(playerName))
            {
                Debug.LogWarning("Player name field cannot be empty");
                return;
            }

            playerNameInputField.text = playerName;
        }

        private void SelectEndlessMode(bool isEnabled)
        {
            if (isEnabled)
            {
                _gameMode = GameMode.Endless;
            }
        }

        private void SelectLevelsMode(bool isEnabled)
        {
            if (isEnabled)
            {
                _gameMode = GameMode.Levels;
            }
        }
        

        private void StartGame()
        {
            if (string.IsNullOrEmpty(playerNameInputField.text))
            {
                playerNameInputField.text = "Guest User";
            }

            Debug.Log("Game mode: " + _gameMode);
            PlayerPrefs.SetString(Constants.PlayerPrefsConstants.GameMode, _gameMode.ToString());
            PlayerPrefs.Save();

            var isNewUser = PlayerDataService.Instance.IsNewUser();
            if (isNewUser)
            {
                PlayerDataService.Instance.CreatePlayerData(playerNameInputField.text);
            }

            SceneManager.LoadScene(Constants.SceneNames.MainScene);
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}
