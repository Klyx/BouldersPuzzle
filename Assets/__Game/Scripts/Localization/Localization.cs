using UnityEngine;

namespace FG {
    [DefaultExecutionOrder(-50)]
    public class Localization : MonoBehaviour {
        public static Localization Instance { get; private set; }

        public Language CurrentLanguage { get; private set; }

        public int CurrentLanguageIndex {
            get => GameSettings.Instance.Language;
            set {
                GameSettings.Instance.Language = value;
                CurrentLanguage = _languages[GameSettings.Instance.Language];
            }
        }

        [SerializeField] private Language[] _languages;

        public bool GetText(in string id, out string text) {
            return CurrentLanguage.GetText(id, out text);
        }

        private void Awake() {
            if (Instance) {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Instance.CurrentLanguage = _languages[GameSettings.Instance.Language];
        }
    }
}