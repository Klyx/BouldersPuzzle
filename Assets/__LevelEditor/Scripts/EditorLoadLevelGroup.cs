using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace FG {
	public class EditorLoadLevelGroup : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI _levelGrouptext;
		[SerializeField] private RadioGroup _radioGroup;
		[SerializeField] private EditorLevels _editorLevels;

		[Header("Level group info")]
		[SerializeField] private TMP_InputField _displayName;
		[SerializeField] private TMP_InputField _creator;
		[SerializeField] private TMP_InputField _version;

		private int _selectedIndex;

		public void OnDisplayNameSet(string text) {
			string filename = $"{GameSettings.Instance.LevelFolder}{_levelGrouptext.text}{Path.AltDirectorySeparatorChar}info.txt";
			File.WriteAllText(filename, $"{_displayName.text}:{_creator.text}:{_version.text}");
		}
		
		public void OnCreatorSet(string text) {
			string filename = $"{GameSettings.Instance.LevelFolder}{_levelGrouptext.text}{Path.AltDirectorySeparatorChar}info.txt";
			File.WriteAllText(filename, $"{_displayName.text}:{_creator.text}:{_version.text}");
		}
		
		public void OnVersionSet(string text) {
			string filename = $"{GameSettings.Instance.LevelFolder}{_levelGrouptext.text}{Path.AltDirectorySeparatorChar}info.txt";
			File.WriteAllText(filename, $"{_displayName.text}:{_creator.text}:{_version.text}");
		}

		public void OnSelectedLevelGroupChanged(int index) {
			_selectedIndex = index;
		}
		public void OnLoadLevelGroup() {
			_levelGrouptext.text = _radioGroup.ButtonText(_selectedIndex);
			
			_editorLevels.ClearLevelButtons();

			List<string> levels = new List<string>();
			LevelUtility.GetLevelNames($"{GameSettings.Instance.LevelFolder}{_levelGrouptext.text}", ref levels);
			foreach (string level in levels) {
				_editorLevels.AddLevelButton(level);
			}

			string filename = $"{GameSettings.Instance.LevelFolder}{_levelGrouptext.text}{Path.AltDirectorySeparatorChar}info.txt";
			if (File.Exists(filename)) {
				string[] lines = File.ReadAllLines(filename);
				if (lines.Length == 0) {
					return;
				}
				
				string[] info = lines[0].Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
				if (info.Length == 3) {
					_displayName.SetTextWithoutNotify(info[0]);
					_creator.SetTextWithoutNotify(info[1]);
					_version.SetTextWithoutNotify(info[2]);
				}
				else {
					_displayName.SetTextWithoutNotify(string.Empty);
					_creator.SetTextWithoutNotify(string.Empty);
					_version.SetTextWithoutNotify("0.0");
				}
			}
			else {
				_displayName.SetTextWithoutNotify(string.Empty);
				_creator.SetTextWithoutNotify(string.Empty);
				_version.SetTextWithoutNotify("0.0");
			}
		}
	}
}
