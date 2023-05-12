using FG.Gridmap;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

#if UNITY_ANDROID
using UnityEngine.Advertisements;
#endif

namespace FG {
    [DefaultExecutionOrder(100), RequireComponent(typeof(CanvasGroup))]
#if UNITY_ANDROID
    public class GameSceneLoadedController : MonoBehaviour, IUnityAdsListener {
#else
    public class GameSceneLoadedController : MonoBehaviour {
#endif
        [SerializeField] private GridMap _gridMap;

        private CanvasGroup _fadeCanvasGroup;
        private const int PlaySceneIndex = 1;

        public void DoLoadScene(int sceneIndex) {
#if UNITY_ANDROID
            if (sceneIndex == PlaySceneIndex && !GameSettings.Instance.BoughtFullVersion) {
                if (Advertisement.IsReady(GameSettings.PlayStorePlacement)) {
                    Advertisement.Show(GameSettings.PlayStorePlacement);
                }
                else {
                    LoadScene.Instance.StartLoadScene(sceneIndex, GetComponent<CanvasGroup>());
                }
            }
            else {
                LoadScene.Instance.StartLoadScene(sceneIndex, _fadeCanvasGroup);
            }
#else
            if (sceneIndex != 5) {
                GameManager.Instance.CameFromEditor = false;
            }

            LoadScene.Instance.StartLoadScene(sceneIndex, _fadeCanvasGroup);
#endif
        }

        private void LoadDone(bool success) {
            LoadScene.Instance.StartFade(0f, _fadeCanvasGroup, FadeDone);
            _gridMap.CreateStartBlock();
        }

        private void FadeDone() {
            GameManager.Instance.CurrentGameState = GameManager.GameState.CanStart;
        }
        
        public void RestartLevel(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }

#if UNITY_ANDROID
            return;
#else
            LoadScene.Instance.StartLoadScene(1, _fadeCanvasGroup);
#endif
        }

#if UNITY_ANDROID
        public void OnUnityAdsReady(string placementId) {
        }

        public void OnUnityAdsDidError(string message) {
        }

        public void OnUnityAdsDidStart(string placementId) {
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult) {
            Advertisement.Load(GameSettings.PlayStorePlacement);

            if (showResult != ShowResult.Finished) {
                return;
            }

            LoadScene.Instance.StartLoadScene(PlaySceneIndex, _fadeCanvasGroup);
        }

        private void OnDestroy() {
            Advertisement.RemoveListener(this);
        }
#endif

        private void Start() {
            GameManager.Instance.CurrentGameState = GameManager.GameState.Loading;

            GridMapUtility.LoadMap(GameManager.Instance.CurrentLevelGroup, GameManager.Instance.CurrentLevel, _gridMap,
                LoadDone);
        }

        private void Awake() {
#if UNITY_ANDROID
            if (!Advertisement.isInitialized) {
                Advertisement.Initialize(GameSettings.PlayStoreID, GameSettings.AdsTestMode);
            }

            Advertisement.AddListener(this);
            Advertisement.Load(GameSettings.PlayStorePlacement);
#endif

            _fadeCanvasGroup = GetComponent<CanvasGroup>();
            if (string.IsNullOrEmpty(GameManager.Instance.CurrentLevelGroup) ||
                string.IsNullOrEmpty(GameManager.Instance.CurrentLevel)) {
                SceneManager.LoadScene(2);
            }
        }
    }
}