using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace FG {
    public class SelectionSlider : MonoBehaviour {
        [Serializable]
        public class SliderEvent : UnityEvent<int, GameObject> {
        }

        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _contentTransform;
        [SerializeField] private TextMeshProUGUI _selectedOptionLabel;
        [SerializeField] private TextMeshProUGUI _selectedOptionNumberLabel;
        [SerializeField] private Button _previousButton;
        [SerializeField] private Button _nextButton;

        [Space] [Header("Events")] public SliderEvent onIndexChanged;

        private int _currentIndex;
        private bool _isScrolling;
        private Coroutine _scrollingRoutine;

        public void SetOptionLabelText(in string text) {
            _selectedOptionLabel.text = text;
        }

        public void SetCurrentIndex(int index, bool raiseEvent = true, bool scrollToImmediately = true) {
            _currentIndex = Mathf.Clamp(index, 0, _contentTransform.childCount - 1);
            _selectedOptionNumberLabel.text = $"{_currentIndex + 1} / {_contentTransform.childCount}";

            if (_isScrolling) {
                StopCoroutine(_scrollingRoutine);
            }

            _scrollingRoutine = StartCoroutine(ScrollToItem(scrollToImmediately));

            SetButtonInteractable();

            if (raiseEvent && _contentTransform.childCount > 0) {
                onIndexChanged.Invoke(_currentIndex, _contentTransform.GetChild(_currentIndex).gameObject);
            }
        }

        public void AddOption(GameObject option) {
            if (!ReferenceEquals(option.transform.parent, _contentTransform)) {
                option.transform.SetParent(_contentTransform);
            }

            _selectedOptionNumberLabel.text = $"{_currentIndex + 1} / {_contentTransform.childCount}";
            SetButtonInteractable();
        }

        public void DestroyAllOptions() {
            for (int i = _contentTransform.childCount - 1; i >= 0; i--) {
                GameObject go = _contentTransform.GetChild(i).gameObject;
                go.SetActive(false);
                go.transform.SetParent(null);
                Destroy(go);
            }

            SetCurrentIndex(0, false, true);
        }

        public void OnPreviousButtonClick() {
            int index = Mathf.Clamp(_currentIndex - 1, 0, _contentTransform.childCount - 1);
            SetCurrentIndex(index, true, false);
        }

        public void OnNextButtonClick() {
            int index = Mathf.Clamp(_currentIndex + 1, 0, _contentTransform.childCount - 1);
            SetCurrentIndex(index, true, false);
        }

        private void SetButtonInteractable() {
            if (_currentIndex == 0) {
                _previousButton.interactable = false;
            }
            else {
                _previousButton.interactable = true;
            }

            if (_currentIndex == _contentTransform.childCount - 1) {
                _nextButton.interactable = false;
            }
            else {
                _nextButton.interactable = true;
            }
        }

        private IEnumerator ScrollToItem(bool scrollToImmediately = false) {
            _isScrolling = true;
            float elapsedTime = 0f;
            float startPosition = _scrollRect.horizontalNormalizedPosition;
            float scrollTime = GameSettings.Instance.ScrollRectTimeToReachItem;
            float horizontalPosition = _currentIndex / (_contentTransform.childCount - 1f);

            if (scrollToImmediately) {
                yield return null;
            }
            else {
                while (elapsedTime < scrollTime) {
                    elapsedTime += Time.deltaTime;
                    float blend = Mathf.Clamp01(elapsedTime / scrollTime);
                    _scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startPosition, horizontalPosition, blend);
                    yield return null;
                }
            }

            _scrollRect.horizontalNormalizedPosition = horizontalPosition;
            _isScrolling = false;
        }
    }
}