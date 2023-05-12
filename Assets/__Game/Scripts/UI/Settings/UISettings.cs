using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FG {
    public class UISettings : SettingOptions {
        [SerializeField] private Toggle _stepsToggle;
        [SerializeField] private Toggle _movesToggle;
        [SerializeField] private Toggle _timeToggle;
        [SerializeField] private Toggle _goalsToggle;
        [SerializeField] private TMP_Dropdown _languageDropdown;

        public override void OnSaveSetting() {
            GameSettings.Instance.ShowStepsUI = _stepsToggle.isOn;
            GameSettings.Instance.ShowMovesUI = _movesToggle.isOn;
            GameSettings.Instance.ShowTimeUI = _timeToggle.isOn;
            GameSettings.Instance.ShowGoalsUI = _goalsToggle.isOn;
            Localization.Instance.CurrentLanguageIndex = _languageDropdown.value;
        }

        private void Awake() {
            _stepsToggle.isOn = GameSettings.Instance.ShowStepsUI;
            _movesToggle.isOn = GameSettings.Instance.ShowMovesUI;
            _timeToggle.isOn = GameSettings.Instance.ShowTimeUI;
            _goalsToggle.isOn = GameSettings.Instance.ShowGoalsUI;
            _languageDropdown.value = GameSettings.Instance.Language;
            
            gameObject.SetActive(false);
        }
    }
}