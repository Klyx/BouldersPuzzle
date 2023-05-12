using UnityEngine;
using UnityEngine.UI;

namespace FG {
	public class AccessibilitySettings : SettingOptions {
		[SerializeField] private Toggle _accessibilityToggle;
		
		public override void OnSaveSetting() {
			GameSettings.Instance.ShowBlockSymbols = _accessibilityToggle.isOn;
		}
		
		private void Awake() {
			_accessibilityToggle.isOn = GameSettings.Instance.ShowBlockSymbols;
			
			gameObject.SetActive(false);
		}
	}
}
