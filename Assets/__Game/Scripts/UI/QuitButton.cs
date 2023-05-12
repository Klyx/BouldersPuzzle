using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FG {
	[RequireComponent(typeof(Button))]
	public class QuitButton : MonoBehaviour {
		public void DoQuit() {
#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
#else
                Application.Quit();
#endif
		}

		private void Awake() {
			Button button = GetComponent<Button>();
			button.onClick.AddListener(DoQuit);
		}
	}
}
