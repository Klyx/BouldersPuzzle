using UnityEngine;
using UnityEngine.UI;

namespace FG {
	public class AudioSettingsOptions : SettingOptions {
		[SerializeField] private Slider _mainSlider;
		[SerializeField] private Slider _musicSlider;
		[SerializeField] private Slider _sfxSlider;
		[SerializeField] private Slider _uiSlider;
		
		public override void OnSaveSetting() {
			GameSettings.Instance.MasterVolume = _mainSlider.value;
			GameSettings.Instance.MusicVolume = _musicSlider.value;
			GameSettings.Instance.SFXVolume = _sfxSlider.value;
			GameSettings.Instance.UIVolume = _uiSlider.value;
		}

		private void Awake() {
			_mainSlider.value = GameSettings.Instance.MasterVolume;
			_musicSlider.value = GameSettings.Instance.MusicVolume;
			_sfxSlider.value = GameSettings.Instance.SFXVolume;
			_uiSlider.value = GameSettings.Instance.UIVolume;
			
			gameObject.SetActive(false);
		}
	}
}
