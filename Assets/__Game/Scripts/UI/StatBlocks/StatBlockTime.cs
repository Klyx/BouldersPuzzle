using System.Collections;
using TMPro;
using UnityEngine;

namespace FG {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class StatBlockTime : MonoBehaviour, IStatBlock {
        [SerializeField] private bool _isHUD;
        [SerializeField] private bool _updateTime;
        
        private float _playTime;
        private bool _isUpdatingTime;
        private Coroutine _timeRoutine;

        private TextMeshProUGUI _text;

        public void UpdateStatBlock() {
            StringUtility.TimeToString(GameManager.Instance.PlayTime, out string time);
            _text.text = time;
        }

        private void OnGameStateChange(GameManager.GameState gameState) {
            if (gameState == GameManager.GameState.Running && !_isUpdatingTime && gameObject.activeInHierarchy) {
                _timeRoutine = StartCoroutine(UpdateTime());
            }
            else {
                if (_isUpdatingTime) {
                    StopCoroutine(_timeRoutine);
                    _isUpdatingTime = false;
                }
            }
        }

        private IEnumerator UpdateTime() {
            _isUpdatingTime = true;
            int timeCount = 0;
            while (true) {
                StringUtility.TimeToString(GameManager.Instance.PlayTime + timeCount, out string time);
                _text.text = time;

                timeCount++;
                yield return new WaitForSeconds(1f);
            }
        }

        private void OnDestroy() {
            GameManager.Instance.PlayTime = 0f;
            GameManager.Instance.StartPlayTime = 0f;
            UpdateStatBlock();

            if (_updateTime) {
                GameManager.Instance.gameStateEvent.RemoveListener(OnGameStateChange);
            }
        }

        private void Awake() {
            _text = GetComponent<TextMeshProUGUI>();
            GameManager.Instance.PlayTime = 0f;
            GameManager.Instance.StartPlayTime = 0f;
            UpdateStatBlock();
            if (_updateTime) {
                GameManager.Instance.gameStateEvent.AddListener(OnGameStateChange);
            }
            
            if (_isHUD) {
                transform.parent.gameObject.SetActive(GameSettings.Instance.ShowTimeUI);
            }
        }
    }
}