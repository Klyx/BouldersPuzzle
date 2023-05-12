using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FG {
    public class RadioGroup : MonoBehaviour {
        public Color unselectedColor = Color.white;
        public Color selectedColor = Color.green;
        public IntEvent onIndexChanged = new IntEvent();

        private Button _previouslySelectedButton;

        public string ButtonText(int index) =>
            transform.GetChild(index).GetComponentInChildren<TextMeshProUGUI>()?.text;

        public void Step(int step) {
            int childCount = transform.childCount;
            int index = ((_previouslySelectedButton.transform.GetSiblingIndex() + step) % childCount + childCount) %
                        childCount;

            OnClick(transform.GetChild(index).GetComponent<Button>());
        }

        public void ClearSelection() {
            _previouslySelectedButton = null;
        }

        public void OnClick(Button button) {
            if (transform.childCount == 0) {
                return;
            }

            if (ReferenceEquals(_previouslySelectedButton, null)) {
                _previouslySelectedButton = transform.GetChild(0).GetComponent<Button>();
            }

            ColorBlock colorBlock = _previouslySelectedButton.colors;
            colorBlock.normalColor = unselectedColor;
            colorBlock.colorMultiplier = 1f;
            _previouslySelectedButton.colors = colorBlock;

            button.enabled = false;
            colorBlock = button.colors;
            colorBlock.normalColor = selectedColor;
            colorBlock.colorMultiplier = 1f;
            button.colors = colorBlock;
            button.enabled = true;
            _previouslySelectedButton = button;

            onIndexChanged.Invoke(button.transform.GetSiblingIndex());
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void OnButtonAdded(Button button) {
            button.onClick.AddListener(() => OnClick(button));
        }

        private void Awake() {
            Button[] buttons = transform.GetComponentsInChildren<Button>();
            foreach (Button button in buttons) {
                button.onClick.AddListener(() => OnClick(button));
            }

            if (buttons.Length > 0) {
                _previouslySelectedButton = buttons[0];
                ColorBlock colorBlock = _previouslySelectedButton.colors;
                colorBlock.normalColor = selectedColor;
                _previouslySelectedButton.colors = colorBlock;
                onIndexChanged.Invoke(_previouslySelectedButton.transform.GetSiblingIndex());
            }
        }
    }
}