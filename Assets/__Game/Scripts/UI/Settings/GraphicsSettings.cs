using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FG {
	public class GraphicsSettings : SettingOptions {
		[SerializeField] private Toggle _fullscreenToggle;
		[SerializeField] private Toggle _optimizeDrawCalls;
		[SerializeField] private TMP_Dropdown _resolutionDropdown;
		[SerializeField] private TMP_Dropdown _vsyncDropdown;
		
		private Resolution[] _resolutions;
		public override void OnSaveSetting() {
			GameSettings.Instance.Fullscreen = _fullscreenToggle.isOn;
			GameSettings.Instance.MergeMeshes = _optimizeDrawCalls.isOn;
			GameSettings.Instance.VSync = _vsyncDropdown.value;
			
#if !UNITY_ANDROID
GameSettings.Instance.Resolution = _resolutionDropdown.value;
#endif
		}

		private void Awake() {
			_fullscreenToggle.isOn = GameSettings.Instance.Fullscreen;
			_optimizeDrawCalls.isOn = GameSettings.Instance.MergeMeshes;
			
			// vSync
			List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
			Localization.Instance.GetText("DontSync", out string text);
			options.Add(new TMP_Dropdown.OptionData(text));
			Localization.Instance.GetText("SyncEvery", out text);
			options.Add(new TMP_Dropdown.OptionData(text));
			Localization.Instance.GetText("SyncSecond", out text);
			options.Add(new TMP_Dropdown.OptionData(text));
			
			_vsyncDropdown.AddOptions(options);

			_vsyncDropdown.value = GameSettings.Instance.VSync;
			_vsyncDropdown.RefreshShownValue();
			
#if !UNITY_ANDROID
			// Resolution
			_resolutions = Screen.resolutions;
			foreach (Resolution resolution in _resolutions) {
				_resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolution.ToString()));
			}

			_resolutionDropdown.value = GameSettings.Instance.Resolution;
			_resolutionDropdown.RefreshShownValue();
#endif
		}
	}
}
