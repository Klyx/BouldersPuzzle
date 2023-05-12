using System.IO;
using UnityEngine;
using TMPro;
using FG.Gridmap;
using UnityEngine.UI;

namespace FG {
    public class LevelEditorMenu : MonoBehaviour {
        [SerializeField] private TMP_InputField _levelGroupField;
        [SerializeField] private TMP_InputField _levelField;
        [SerializeField] private Button _playButton;
        [SerializeField] private GridMap _gridMap;
        [SerializeField] private SceneLoadedController _sceneLoadedController;

        public void LoadClick() {
            _gridMap.ClearMap();
            _playButton.interactable = GridMapUtility.LoadMap(_levelGroupField.text, _levelField.text, _gridMap, null, true);
        }

        public void SaveFile() {
            GridMapUtility.SaveMap(_levelGroupField.text, _levelField.text, _gridMap);
            _playButton.interactable = true;
        }

        public void ClearMap() {
            _gridMap.ClearMap();
        }

        public void PlayLevel() {
            string levelFilePath =
                $"{GameSettings.Instance.LevelFolder}{_levelGroupField.text}{Path.AltDirectorySeparatorChar}{_levelField.text}.pmap";

            int levelIndex = LevelUtility.GetLevelIndex(_levelGroupField.text, _levelField.text);
            if (levelIndex >= 0 && File.Exists(levelFilePath)) {
                GameManager.Instance.CurrentLevelGroup = _levelGroupField.text;
                GameManager.Instance.CurrentLevel = _levelField.text;
                
                GameManager.Instance.CurrentLevelIndex = levelIndex;
                GameManager.Instance.CameFromEditor = true;
                _sceneLoadedController.DoLoadScene(1);
            }
        }
    }
}
