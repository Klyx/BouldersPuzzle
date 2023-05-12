using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FG {
    [DefaultExecutionOrder(-50)]
    public class GameManager : MonoBehaviour {
        public enum GameState {
            Loading,
            CanStart,
            Running,
            Paused,
            GameOver,
            GameWon
        }

        public GameStateEvent gameStateEvent;

        private GameState _currentGameState = GameState.Loading;

        public static GameManager Instance { get; private set; }

        public string CurrentLevelGroup { get; set; }
        public string CurrentLevel { get; set; }
        public int CurrentLevelIndex { get; set; }
        
        public int Steps { get; set; }
        public int Moves { get; set; }

        public float StartPlayTime { get; set; }
        public float PlayTime { get; set; }

        public bool CameFromEditor { get; set; } = false;
        

        public GameState CurrentGameState {
            get => _currentGameState;
            set
            {
                if (_currentGameState == GameState.CanStart && value == GameState.Running) {
                    StartPlayTime = Time.time;
                    PlayTime = 0;
                } 
                else if (_currentGameState == GameState.Running && value != GameState.Running) {
                    PlayTime += Time.time - StartPlayTime;
                }
                else if (_currentGameState == GameState.Paused && value == GameState.Running) {
                    StartPlayTime = Time.time;
                }
                else {
                    StartPlayTime = 0f;
                    PlayTime = 0;
                }
                
                _currentGameState = value;
                gameStateEvent.Invoke(_currentGameState);
            }
        }

        public string CurrentLevelSavePath => $"{GameSettings.Instance.LevelFolder}{CurrentLevelGroup}{Path.AltDirectorySeparatorChar}{CurrentLevel}.sav";
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            Steps = 0;
            Moves = 0;
            StartPlayTime = 0f;
            PlayTime = 0f;
        }

        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Awake() {
            if (Instance) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
    }
}