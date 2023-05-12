using UnityEngine;
using UnityEngine.UI;

namespace FG {
    [RequireComponent(typeof(Button))]
    public class PrevNextLevelButton : MonoBehaviour {
        public bool isNextButton;

        private int _levelCount;

        public void OnButtonClick() {
            int index = GameManager.Instance.CurrentLevelIndex;
            if (isNextButton) {
                index++;
            }
            else {
                index--;
            }
            
            index = Mathf.Clamp(index, 0, _levelCount - 1);
            LevelUtility.GetLevelName(GameManager.Instance.CurrentLevelGroup, index, out string levelName);
            GameManager.Instance.CurrentLevelIndex = index;
            GameManager.Instance.CurrentLevel = levelName;
        }

        private void Awake() {
            Button button = GetComponent<Button>();
            _levelCount = LevelUtility.CountLevelsInGroup(
                $"{GameSettings.Instance.LevelFolder}{GameManager.Instance.CurrentLevelGroup}");

            if (isNextButton) {
                if (GameManager.Instance.CurrentLevelIndex == _levelCount - 1) {
                    button.interactable = false;
                }
            }
            else {
                if (GameManager.Instance.CurrentLevelIndex == 0) {
                    button.interactable = false;
                }
            }
        }
    }
}