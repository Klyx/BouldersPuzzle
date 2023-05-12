using System;
using TMPro;
using UnityEngine;

namespace FG {
	[RequireComponent(typeof(CanvasGroup))]
	public class FadeCanvasGroupAddString : MonoBehaviour {
		private CanvasGroup _canvasGroup;
		private TMP_InputField _inputField;
		private Action<string> _onFadeDone;

		public void FadeInMenu(Action<string> onFadeDone) {
			_onFadeDone = onFadeDone;
			LoadScene.Instance.StartFade(1f, _canvasGroup, FadeInDone);
		}
		
		public void FadeOutMenu() {
			LoadScene.Instance.StartFade(0f, _canvasGroup, FadeOutDone);
		}

		private void FadeInDone() {
			_canvasGroup.interactable = true;
			_canvasGroup.blocksRaycasts = true;
		}
		
		private void FadeOutDone() {
			_onFadeDone?.Invoke(_inputField.text);
			_canvasGroup.interactable = false;
			_canvasGroup.blocksRaycasts = false;
			_inputField.text = string.Empty;
		}

		private void Awake() {
			_canvasGroup = GetComponent<CanvasGroup>();
			_inputField = GetComponentInChildren<TMP_InputField>();
		}
	}
}
