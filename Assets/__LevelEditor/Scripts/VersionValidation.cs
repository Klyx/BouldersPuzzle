using UnityEngine;

namespace FG {
	[CreateAssetMenu(fileName = "Input Field Validator", menuName = "Input Field Validator")]
	public class VersionValidation : TMPro.TMP_InputValidator {
		public override char Validate(ref string text, ref int pos, char ch) {
			if (char.IsNumber(ch) || ch == '.') {
				text = text.Insert(pos, ch.ToString());
				pos++;
			}
			return '\0';
		}
	}
}
