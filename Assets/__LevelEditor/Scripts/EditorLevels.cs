using System.IO;
using FG.Gridmap;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FG {
	[DefaultExecutionOrder(-50)]
	public class EditorLevels : MonoBehaviour {
		[SerializeField] private Transform _contentTransform;
		[SerializeField] private Button _contentButonPrefab;
		[SerializeField] private GridMap _map;
		[SerializeField] private TextMeshProUGUI _levelGroupText;
		[SerializeField] private TextMeshProUGUI _levelText;
		[SerializeField] private EditorLayers _editorLayers;
		[SerializeField] private Button _loadLevelButton;
		[Header("Events")]
		[SerializeField] private UnityEvent onNewLevel;
		
		private int _selectedIndex;
		private RadioGroup _radioGroup;

		public void OnSelectedLevelChanged(int index) {
			_loadLevelButton.interactable = true;
			_selectedIndex = index;
		}

		public void LoadLevel() {
			GridMapUtility.LoadMap(_levelGroupText.text, _radioGroup.ButtonText(_selectedIndex), _map, LoadLevelDone);
			_editorLayers.ClearNodes();
			_editorLayers.AddNodesFromMap(_map);
		}

		public void StartAddLevel(FadeCanvasGroupAddString fadeCanvasGroupAddString) {
			fadeCanvasGroupAddString.FadeInMenu(AddLevelDone);
		}

		public void ClearLevelButtons() {
			_loadLevelButton.interactable = false;
			foreach (Transform child in _contentTransform) {
				Destroy(child.gameObject);
			}
			_radioGroup.ClearSelection();
		}
		
		public void AddLevelButton(in string level) {
			TextMeshProUGUI textMesh = Instantiate(_contentButonPrefab, _contentTransform)?.GetComponentInChildren<TextMeshProUGUI>();
			if (!ReferenceEquals(textMesh, null)) {
				textMesh.text = level;
			}
			_radioGroup.OnButtonAdded(textMesh.transform.parent.GetComponent<Button>());
		}

		private void LoadLevelDone(bool wasLoded) {
			if (wasLoded) {
				_levelText.text = _radioGroup.ButtonText(_selectedIndex);
			}
		}
		
		private void AddLevelDone(string text) {
			if (string.IsNullOrEmpty(text)) {
				return;
			}

			text = text.ToLower();
			if (!File.Exists($"{GameSettings.Instance.LevelFolder}{text}")) {
				AddLevelButton(text);
				_map.ClearMap();
				_levelText.text = text;
				onNewLevel.Invoke();
			}
		}

		private void Awake() {
			_radioGroup = _contentTransform.GetComponent<RadioGroup>();
		}
	}
}
