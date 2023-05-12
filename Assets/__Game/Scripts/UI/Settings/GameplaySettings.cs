using UnityEngine;
using UnityEngine.UI;

namespace FG {
	public class GameplaySettings : SettingOptions {
		[SerializeField] private Toggle _CameraShakeToggle;
		
		public override void OnSaveSetting() {
			GameSettings.Instance.ShakeCamera = _CameraShakeToggle.isOn;
		}
		
		private void Awake() {
			_CameraShakeToggle.isOn = GameSettings.Instance.ShakeCamera;
			
			gameObject.SetActive(false);
		}
	}
}
