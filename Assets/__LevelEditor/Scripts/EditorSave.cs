using FG.Gridmap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FG {
	[RequireComponent(typeof(Button))]
	public class EditorSave : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI _levelGroupText;
		[SerializeField] private TextMeshProUGUI _levelText;
		[SerializeField] private GridMap _map;
		[SerializeField] private Image _panel;
		[SerializeField] private Button _playButton;

		private Button _button;

		public void OnPerformSave() {
			GridMapUtility.SaveMap(_levelGroupText.text, _levelText.text, _map);
			if (ColorUtility.TryParseHtmlString("#66FF6A", out Color color)) {
				_panel.color = color;
			}
			
			_button.interactable = false;
			_playButton.interactable = true;
		}

		public void OnBlocksChanged(GridMap map, Block block, bool added, Vector3Int position) {
			if (ColorUtility.TryParseHtmlString("#FF667C", out Color color)) {
				_panel.color = color;
			}

			_button.interactable = true;
			_playButton.interactable = false;
		}

		private void Awake() {
			_button = GetComponent<Button>();
		}
	}
}
