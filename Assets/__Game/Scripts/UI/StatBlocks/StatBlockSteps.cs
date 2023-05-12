using TMPro;
using UnityEngine;

namespace FG {
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class StatBlockSteps : MonoBehaviour, IStatBlock {
		[SerializeField] private bool _isHUD;
		
		private TextMeshProUGUI _text;

		public void UpdateStatBlock() {
			_text.text = GameManager.Instance.Steps.ToString();
		}

		private void Awake() {
			_text = GetComponent<TextMeshProUGUI>();
			
			if (_isHUD) {
				transform.parent.gameObject.SetActive(GameSettings.Instance.ShowStepsUI);
			}
		}
	}
}
