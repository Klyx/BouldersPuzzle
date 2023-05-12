using UnityEngine;

namespace FG {
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeCanvasGroup : MonoBehaviour {
        private CanvasGroup _canvasGroup;

        public void FadeInMenu() {
            LoadScene.Instance.StartFade(1f, _canvasGroup, OnFadeDone);
        }

        public void FadeOutMenu() {
            LoadScene.Instance.StartFade(0f, _canvasGroup, OnFadeDone);
        }

        private void OnFadeDone() {
            if (_canvasGroup.alpha > 0.9f) {
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }
            else {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }
        }

        private void Start() {
            FadeOutMenu();
        }

        private void Awake() {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
    }
}