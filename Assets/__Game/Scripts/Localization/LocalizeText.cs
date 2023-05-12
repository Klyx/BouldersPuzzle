using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace FG {
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LocalizeText : MonoBehaviour {
		[SerializeField] private string _id;
		[SerializeField] private bool _isTitle;

		private void Start() {
			TextMeshProUGUI textMesh = GetComponent<TextMeshProUGUI>();
			Assert.IsNotNull(textMesh, "text != null");

			if (_isTitle) {
				Localization.Instance.CurrentLanguage.SetTitleFont(ref textMesh);
			}
			
			Localization.Instance.CurrentLanguage.GetText(_id, out string text);
			textMesh.text = text;
		}
	}
}
