using UnityEngine;
using UnityEngine.UI;

namespace FG {
	public class ControlsSettings : SettingOptions {
		[SerializeField] private Toggle _flipCameraToggle;
		
		public override void OnSaveSetting() {
			GameSettings.Instance.CameraInvertLook = _flipCameraToggle.isOn ? -1f : 1f;
		}
		
		private void Awake() {
			_flipCameraToggle.isOn = GameSettings.Instance.CameraInvertLook < 0f;
			
			gameObject.SetActive(false);
		}
	}
}
