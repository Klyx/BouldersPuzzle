using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using UnityEngine.SceneManagement;

namespace FG {
    [DefaultExecutionOrder(-100)]
    public class LoadScene : MonoBehaviour {
        public static LoadScene Instance { get; private set; }

        /// <summary>
        /// Call this to fade in and then load the next scene.
        /// </summary>
        /// <param name="buildIndex"></param>
        public void StartLoadScene(int sceneIndex, CanvasGroup canvasGroup) {
            Assert.IsNotNull(canvasGroup, "canvasGroup != null");
            StartCoroutine(PerformFade(1f, canvasGroup, sceneIndex));
        }

        /// <summary>
        /// Call this to fade out.
        /// </summary>
        public void OnSceneLoadedDone(CanvasGroup canvasGroup) {
            Assert.IsNotNull(canvasGroup, "canvasGroup != null");
            StartCoroutine(PerformFade(0f, canvasGroup));
        }

        public void StartFade(float targetAlpha, CanvasGroup canvasGroup, Action onFadeDone = null) {
            Assert.IsNotNull(canvasGroup, "canvasGroup != null");
            StartCoroutine(PerformFade(targetAlpha, canvasGroup, -1, onFadeDone));
        }

        private IEnumerator PerformFade(float targetAlpha, CanvasGroup canvasGroup, int loadScene = -1,
            Action onFadeDone = null) {
            float elapsedTime = 0f;
            float currentAlpha = canvasGroup.alpha;
            float startAlpha = currentAlpha;
            float fadeTime = GameSettings.Instance.SceneFadeTime;

            while (elapsedTime < fadeTime) {
                elapsedTime += Time.deltaTime;
                float blend = Mathf.Clamp01(elapsedTime / fadeTime);
                currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, blend);
                canvasGroup.alpha = currentAlpha;
                yield return null;
            }

            if (loadScene < 0) {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                onFadeDone?.Invoke();
            }
            else {
                SceneManager.LoadScene(loadScene);
            }
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