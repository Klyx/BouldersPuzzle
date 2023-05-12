using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace FG {
    [RequireComponent(typeof(SelectionSlider))]
    public class LevelGroupSelection : MonoBehaviour {
        [SerializeField] private GameObject _levelGroupInfoPrefab;
        [SerializeField] private Transform _contentTransform;
        
        private SelectionSlider _groupSlider;
        
        public void OnLevelGroupChanged(int index, GameObject option) {
            LevelGroupInfoUI groupInfoUI = option.GetComponent<LevelGroupInfoUI>();
            Assert.IsNotNull(groupInfoUI, "groupInfo != null");
            _groupSlider.SetOptionLabelText(groupInfoUI.Name);
            GameSave.Instance.LastSelectedLevelGroupIndex = index;
            GameManager.Instance.CurrentLevelGroup = groupInfoUI.FolderName;
        }

        private void Start() {
            List<string> levelGroups = new List<string>();
            LevelGroupUtility.GetLevelGroupFolders(out levelGroups);

            foreach (string levelGroup in levelGroups) {
                LevelGroupInfoUI groupInfoUI = Instantiate(_levelGroupInfoPrefab, _contentTransform)
                    ?.GetComponent<LevelGroupInfoUI>();
                groupInfoUI.FolderName = levelGroup;
                LevelGroupUtility.GetLevelGroupInfo(levelGroup, ref groupInfoUI);
                _groupSlider.AddOption(groupInfoUI.gameObject);
            }
            
            _groupSlider.SetCurrentIndex(GameSave.Instance.LastSelectedLevelGroupIndex, true);
        }

        private void Awake() {
            _groupSlider = GetComponent<SelectionSlider>();
        }
    }
}