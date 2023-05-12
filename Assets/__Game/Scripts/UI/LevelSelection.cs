using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

namespace FG {
    [RequireComponent(typeof(SelectionSlider))]
    public class LevelSelection : MonoBehaviour {
        [SerializeField] private GameObject _levelInfoPrefab;
        [SerializeField] private Transform _contentTransform;

        [Header("Labels")] [SerializeField] private TextMeshProUGUI _optionValueLabel;

        private SelectionSlider _groupSlider;

        public void OnLevelGroupChanged(int index, GameObject option) {
            ClearLevelInfo();
            AddLevelInfo(option);
        }

        public void OnLevelChanged(int index, GameObject option) {
            LevelInfoUI levelInfo = option.GetComponent<LevelInfoUI>();
            _optionValueLabel.text = levelInfo.Name;
            GameSave.Instance.SetLastSelectedLevelOfGroup(levelInfo.LevelFolder, index);
            GameManager.Instance.CurrentLevel = _optionValueLabel.text;
            GameManager.Instance.CurrentLevelIndex = index;
            
            if (!levelInfo.InfoSet &&
                GameSave.Instance.GetLevelInfo(GameManager.Instance.CurrentLevelSavePath,
                    out GameSave.LevelPlayInfo levelPlayInfo)) {
                levelInfo.TimesPlayed = levelPlayInfo.numberOfPlays;
                levelInfo.Steps = levelPlayInfo.numberOfSteps;
                levelInfo.Moves = levelPlayInfo.numberOfMoves;
                levelInfo.Time = levelPlayInfo.playTime;
                levelInfo.InfoSet = true;
            }
        }

        private void AddLevelInfo(GameObject groupObject) {
            LevelGroupInfoUI groupInfo = groupObject.GetComponent<LevelGroupInfoUI>();
            LevelUtility.GetLevelFilenames($"{GameSettings.Instance.LevelFolder}{groupInfo.FolderName}",
                out List<string> levelFilenames);
            foreach (string levelFile in levelFilenames) {
                LevelInfoUI levelInfo = Instantiate(_levelInfoPrefab, _contentTransform)?.GetComponent<LevelInfoUI>();
                levelInfo.LevelFolder = groupInfo.FolderName;
                levelInfo.Name = Path.GetFileNameWithoutExtension(levelFile);
                _groupSlider.AddOption(levelInfo.gameObject);
            }

            if (GameSave.Instance.GetLastSelectedLevelOfGroup(groupInfo.FolderName, out int lastIndex)) {
                _groupSlider.SetCurrentIndex(0, false, true);
                _groupSlider.SetCurrentIndex(lastIndex, true, true);
            }
            else {
                _groupSlider.SetCurrentIndex(0, true, true);
                GameSave.Instance.SetLastSelectedLevelOfGroup(groupInfo.FolderName, 0);
            }
        }

        private void ClearLevelInfo() {
            // Todo: Reuse widgets.
            _groupSlider.DestroyAllOptions();
        }

        private void Awake() {
            _groupSlider = GetComponent<SelectionSlider>();
        }
    }
}