using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FG {
	[DefaultExecutionOrder(-50)]
	public class EditorLevelGroup : MonoBehaviour {
		[SerializeField] private Transform _contentTransform;
		[SerializeField] private Button _contentButonPrefab;
		[SerializeField] private TextMeshProUGUI _text;

		public void OpenLevelGroupFolder() {
			Application.OpenURL($"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}Levels{Path.AltDirectorySeparatorChar}");
		}

		public void StartAddLevelGroup(FadeCanvasGroupAddString fadeCanvasGroupAddString) {
			fadeCanvasGroupAddString.FadeInMenu(AddLevelGroupDone);
		}

		private void AddLevelGroupButton(in string levelGroup) {
			TextMeshProUGUI textMesh = Instantiate(_contentButonPrefab, _contentTransform)?.GetComponentInChildren<TextMeshProUGUI>();
			if (!ReferenceEquals(textMesh, null)) {
				textMesh.text = levelGroup;
			}
		}

		private void AddLevelGroupDone(string text) {
			if (string.IsNullOrEmpty(text)) {
				return;
			}

			text = text.ToLower();
			if (!Directory.Exists($"{GameSettings.Instance.LevelFolder}{text}")) {
				Directory.CreateDirectory($"{GameSettings.Instance.LevelFolder}{text}");
				_text.text = text;
				AddLevelGroupButton(text);
			}
		}

		private void Awake() {
			LevelGroupUtility.GetLevelGroupFolders(out List<string> levelGroups);
			foreach (string levelGroup in levelGroups) {
				AddLevelGroupButton(levelGroup);
			}
		}
	}
}
