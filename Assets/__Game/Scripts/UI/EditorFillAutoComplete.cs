using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace FG {
    public class EditorFillAutoComplete : MonoBehaviour {
        [SerializeField] private AutoComplete _levelGroupAutoComplete;
        [SerializeField] private AutoComplete _levelsAutoComplete;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _sendButton;

        private string _levelGroup;
        private string _level;

        public void OnLevelGroupSet(string levelGroup) {
            if (string.IsNullOrEmpty(levelGroup) || string.IsNullOrWhiteSpace(levelGroup)) {
                _loadButton.interactable = false;
                _saveButton.interactable = false;
                _playButton.interactable = false;
                _sendButton.interactable = false;
                _levelsAutoComplete.options.Clear();
                return;
            }
            
            _levelGroup = levelGroup;
            _loadButton.interactable = DoesLevelfileExist();
            _saveButton.interactable = CanSave();
            _sendButton.interactable = true;
            
            _levelsAutoComplete.options.Clear();
            LevelUtility.GetLevelNames($"{GameSettings.Instance.LevelFolder}{_levelGroup}", ref _levelsAutoComplete.options);
        }

        public void OnLevelSet(string level) {
            if (string.IsNullOrEmpty(level) || string.IsNullOrWhiteSpace(level)) {
                _loadButton.interactable = false;
                _saveButton.interactable = false;
                _playButton.interactable = false;
                return;
            }
            
            _level = level;
            _loadButton.interactable = DoesLevelfileExist();
            _saveButton.interactable = CanSave();
        }

        private bool DoesLevelfileExist() {
            string levelFilePath =
                $"{GameSettings.Instance.LevelFolder}{_levelGroup}{Path.AltDirectorySeparatorChar}{_level}.pmap";
            return File.Exists(levelFilePath);
        }

        private bool CanSave() {
            return !string.IsNullOrEmpty(_level) || !string.IsNullOrWhiteSpace(_level);
        }

        private void Awake() {
            LevelGroupUtility.GetLevelGroupNames(ref _levelGroupAutoComplete.options);
        }
    }
}