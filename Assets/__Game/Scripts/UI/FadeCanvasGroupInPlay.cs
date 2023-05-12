using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FG {
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeCanvasGroupInPlay : MonoBehaviour {
        public bool allowFadeWhenGameOver = false;
        public bool allowStateChange = false;
        public GameObject setSelectedObjectOnEnabled;
        public BoolEvent startFadeEvent;

        private CanvasGroup _canvasGroup;

        public bool IsFading { get; private set; }

        private Coroutine _fadeRoutine;

        public void FadeToEnabled(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }

            if (GameManager.Instance.CurrentGameState == GameManager.GameState.Paused ||
                (GameManager.Instance.CurrentGameState == GameManager.GameState.CanStart && _canvasGroup.alpha > 0.9f)) {
                if (allowStateChange) {
                    FadeToDisabled();
                    return;
                }
            }

            if (_canvasGroup.alpha > 0.9f || (!allowFadeWhenGameOver &&
                                              (GameManager.Instance.CurrentGameState == GameManager.GameState.GameWon ||
                                               GameManager.Instance.CurrentGameState ==
                                               GameManager.GameState.GameOver))) {
                return;
            }

            if (IsFading) {
                StopCoroutine(_fadeRoutine);
            }

            if (GameManager.Instance.CurrentGameState == GameManager.GameState.Running) {
                GameManager.Instance.CurrentGameState = GameManager.GameState.Paused;
            }

            startFadeEvent.Invoke(true);
            _fadeRoutine = StartCoroutine(PerformFade(1f));
        }

        public void FadeToDisabled() {
            if ((GameManager.Instance.CurrentGameState == GameManager.GameState.Paused ||
                 GameManager.Instance.CurrentGameState == GameManager.GameState.CanStart) && _canvasGroup.alpha < 0.1f) {
                if (!allowStateChange) {
                    startFadeEvent.Invoke(true);
                    _fadeRoutine = StartCoroutine(PerformFade(1f));
                    return;
                }
            }
            
            if (IsFading) {
                StopCoroutine(_fadeRoutine);
            }

            startFadeEvent.Invoke(false);
            _fadeRoutine = StartCoroutine(PerformFade(0f));
        }

        private void OnGameStateChanged(GameManager.GameState gameState) {
            if (allowFadeWhenGameOver &&
                (gameState == GameManager.GameState.GameWon || gameState == GameManager.GameState.GameOver)) {
                startFadeEvent.Invoke(true);
                _fadeRoutine = StartCoroutine(PerformFade(1f));
            }
        }

        private IEnumerator PerformFade(float targetAlpha) {
            IsFading = true;

            float elapsedTime = 0f;
            float currentAlpha = _canvasGroup.alpha;
            float startAlpha = currentAlpha;
            float fadeTime = GameSettings.Instance.MenuFadeTime;

            while (elapsedTime < fadeTime) {
                elapsedTime += Time.deltaTime;
                float blend = Mathf.Clamp01(elapsedTime / fadeTime);
                currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, blend);
                _canvasGroup.alpha = currentAlpha;
                yield return null;
            }

            if (targetAlpha >= 0.9f) {
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
                if (allowStateChange) {
                    EventSystem.current.SetSelectedGameObject(setSelectedObjectOnEnabled);
                    IsFading = false;
                    yield break;
                }
            }
            else if (targetAlpha < 0.1f) {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
                if (allowStateChange && GameManager.Instance.CurrentGameState == GameManager.GameState.Paused) {
                    GameManager.Instance.CurrentGameState = GameManager.GameState.Running;
                }
            }
            
            EventSystem.current.SetSelectedGameObject(null);

            IsFading = false;
        }

        private void OnDisable() {
            GameManager.Instance.gameStateEvent.RemoveListener(OnGameStateChanged);
        }

        private void Awake() {
            _canvasGroup = GetComponent<CanvasGroup>();

            if (allowFadeWhenGameOver) {
                GameManager.Instance.gameStateEvent.AddListener(OnGameStateChanged);
            }
        }
    }
}