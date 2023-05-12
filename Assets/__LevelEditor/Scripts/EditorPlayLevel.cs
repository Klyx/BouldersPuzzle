using System.IO;
using TMPro;
using UnityEngine;

namespace FG {
	public class EditorPlayLevel : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI _editorLevelGroupText;
		[SerializeField] private TextMeshProUGUI _editorLevelText;
		[SerializeField] private SceneLoadedController _sceneLoadedController;
		
		public void OnPlayLevel() {
			string levelFilePath =
				$"{GameSettings.Instance.LevelFolder}{_editorLevelGroupText.text}{Path.AltDirectorySeparatorChar}{_editorLevelText.text}.pmap";

			 int levelIndex = LevelUtility.GetLevelIndex(_editorLevelGroupText.text, _editorLevelText.text);
			 if (levelIndex >= 0 && File.Exists(levelFilePath)) {
			 	GameManager.Instance.CurrentLevelGroup = _editorLevelGroupText.text;
			 	GameManager.Instance.CurrentLevel = _editorLevelText.text;
                 
			 	GameManager.Instance.CurrentLevelIndex = levelIndex;
			 	GameManager.Instance.CameFromEditor = true;
			 	_sceneLoadedController.DoLoadScene(1);
			}
		}
	}
}
