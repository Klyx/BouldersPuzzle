using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_ANDROID
using UnityEngine.Advertisements;
#endif

namespace FG {
    [RequireComponent(typeof(CanvasGroup)), DefaultExecutionOrder(1000)]
#if UNITY_ANDROID
    public class SceneLoadedController : MonoBehaviour, IUnityAdsListener {
#else
    public class SceneLoadedController : MonoBehaviour {
#endif
        private const int PlaySceneIndex = 1;

        public void DoLoadScene(int sceneIndex) {
#if UNITY_ANDROID
            if (sceneIndex == PlaySceneIndex) {
                if (GameSettings.Instance.FirstPlayThisSession || GameSettings.Instance.BoughtFullVersion) {
                    GameSettings.Instance.FirstPlayThisSession = false;
                    LoadScene.Instance.StartLoadScene(sceneIndex, GetComponent<CanvasGroup>());
                }
                else {
                    if (Advertisement.IsReady(GameSettings.PlayStorePlacement)) {
                        Advertisement.Show(GameSettings.PlayStorePlacement);
                    }
                    else {
                        LoadScene.Instance.StartLoadScene(sceneIndex, GetComponent<CanvasGroup>());
                    }
                }
            }
            else {
                LoadScene.Instance.StartLoadScene(sceneIndex, GetComponent<CanvasGroup>());
            }
#else
			LoadScene.Instance.StartLoadScene(sceneIndex, GetComponent<CanvasGroup>());
#endif
        }
        
        public void DoLoadMainMenu(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }
            
#if UNITY_ANDROID
            return;
            // if (sceneIndex == PlaySceneIndex) {
            //     if (GameSettings.Instance.FirstPlayThisSession || GameSettings.Instance.BoughtFullVersion) {
            //         GameSettings.Instance.FirstPlayThisSession = false;
            //         LoadScene.Instance.StartLoadScene(sceneIndex, GetComponent<CanvasGroup>());
            //     }
            //     else {
            //         if (Advertisement.IsReady(GameSettings.PlayStorePlacement)) {
            //             Advertisement.Show(GameSettings.PlayStorePlacement);
            //         }
            //         else {
            //             LoadScene.Instance.StartLoadScene(sceneIndex, GetComponent<CanvasGroup>());
            //         }
            //     }
            // }
            // else {
            //     LoadScene.Instance.StartLoadScene(sceneIndex, GetComponent<CanvasGroup>());
            // }
#else
            LoadScene.Instance.StartLoadScene(0, GetComponent<CanvasGroup>());
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

            LoadScene.Instance.StartLoadScene(PlaySceneIndex, GetComponent<CanvasGroup>());
        }

        private void OnDestroy() {
            Advertisement.RemoveListener(this);
        }

        private void Awake() {
            if (!Advertisement.isInitialized) {
                Advertisement.Initialize(GameSettings.PlayStoreID, GameSettings.AdsTestMode);
            }
            
            Advertisement.AddListener(this);
            Advertisement.Load(GameSettings.PlayStorePlacement);
        }
#endif
        
        private void Start() {
            LoadScene.Instance.OnSceneLoadedDone(GetComponent<CanvasGroup>());
        }
    }
}