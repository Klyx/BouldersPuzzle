using FG.Gridmap;
using TMPro;
using UnityEngine;

namespace FG {
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class StatBlockMoves : MonoBehaviour, IStatBlock {
		[SerializeField] private bool _isHUD;
		
		private TextMeshProUGUI _text;

		public void UpdateStatBlock() {
			_text.text = GameManager.Instance.Moves.ToString();
		}

		private void Awake() {
			_text = GetComponent<TextMeshProUGUI>();
			
			if (_isHUD) {
				transform.parent.gameObject.SetActive(GameSettings.Instance.ShowMovesUI);
			}
		}
	}
}
