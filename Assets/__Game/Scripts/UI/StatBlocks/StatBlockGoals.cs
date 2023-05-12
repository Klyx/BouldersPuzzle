using FG.Gridmap;
using TMPro;
using UnityEngine;

namespace FG {
	[RequireComponent(typeof(TextMeshProUGUI)), DefaultExecutionOrder(1000)]
	public class StatBlockGoals : MonoBehaviour, IStatBlock {
		[SerializeField] private bool _isHUD;
		
		private TextMeshProUGUI _text;

		public void UpdateStatBlock() {
			_text.text = $"{GoalBlock.ConnectedGoals} / {GoalBlock.GoalCount}";
		}

		private void Start() {
			UpdateStatBlock();
		}

		private void Awake() {
			_text = GetComponent<TextMeshProUGUI>();

			if (_isHUD) {
				transform.parent.gameObject.SetActive(GameSettings.Instance.ShowGoalsUI);
			}
		}
	}
}
