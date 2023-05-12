using UnityEngine;
using UnityEngine.Audio;

namespace FG {
	public class SaveSettingsUI : MonoBehaviour {
		[SerializeField] private AudioMixer _audioMixer;
		
		public void SaveSettings() {
			SettingOptions[] settingOptions = FindObjectsOfType<SettingOptions>(true);
			foreach (SettingOptions settingOption in settingOptions) {
				settingOption.OnSaveSetting();
			}
			
			GameSettings.SaveSettings();
			GameSettings.Instance.ApplySettings();
		}
	}
}
