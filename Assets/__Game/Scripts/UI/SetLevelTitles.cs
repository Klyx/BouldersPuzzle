using TMPro;
using UnityEngine;

namespace FG {
	public class SetLevelTitles : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI _levelGroupText;
		[SerializeField] private TextMeshProUGUI _levelText;

		private void Awake() {
			_levelGroupText.text = GameManager.Instance.CurrentLevelGroup;
			_levelText.text = GameManager.Instance.CurrentLevel;
		}
	}
}
