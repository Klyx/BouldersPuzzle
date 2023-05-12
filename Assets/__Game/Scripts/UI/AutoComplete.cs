using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace FG {
    [RequireComponent(typeof(TMP_InputField))]
    public class AutoComplete : MonoBehaviour {
        [NonSerialized] public List<string> options = new List<string>();

        private TMP_InputField _inputField;
        private string _lastInput = string.Empty;

        private void OnTextChanged(string input) {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input)) {
                return;
            }

            if (input.Length > _lastInput.Length) {
                input = input.ToLower();
                string text = options.LastOrDefault((str) => str.ToLower().Contains(input));

                if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text)) {
                    return;
                }

                _inputField.SetTextWithoutNotify(text);
                _inputField.Select();
                _inputField.MoveTextEnd(true);

                _lastInput = input;
            }
        }

        private void Awake() {
            _inputField = GetComponent<TMP_InputField>();
            _inputField.onValueChanged.AddListener(OnTextChanged);
        }

        private void OnDestroy() {
            _inputField.onEndEdit.RemoveListener(OnTextChanged);
        }
    }
}