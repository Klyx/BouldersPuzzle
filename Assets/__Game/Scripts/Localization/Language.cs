using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace FG {
	[CreateAssetMenu(fileName = "ButtonSetting", menuName = "FG/Localization/Language")]
	public class Language : ScriptableObject {
		[SerializeField] private TMP_FontAsset titleFontAsset;
		
		[Serializable]
		public struct LocalizedText {
			public string id;
			[Multiline] public string text;
		}

		[SerializeField] private List<LocalizedText> _texts = new List<LocalizedText>();

		public bool GetText(string id, out string text) {
			int index = _texts.FindIndex(item => item.id.Equals(id));
			Assert.IsTrue(index >= 0, $"index >= 0:\tFailed to find {id}");
			if (index >= 0) {
				text = _texts[index].text;
				return true;
			}

			text = string.Empty;
			return false;
		}

		public void SetTitleFont(ref TextMeshProUGUI textMesh) {
			Assert.IsNotNull(textMesh, "textMesh != null");
			if (titleFontAsset) {
				textMesh.font = titleFontAsset;
			}
		}
	}
}
