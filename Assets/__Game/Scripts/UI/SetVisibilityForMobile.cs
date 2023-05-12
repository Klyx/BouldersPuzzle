using UnityEngine;

namespace FG {
	[DefaultExecutionOrder(100)]
	public class SetVisibilityForMobile : MonoBehaviour {
		[SerializeField] private bool _visibilityForMobile;
		private void Awake() {
			if (Application.isMobilePlatform || Application.platform == RuntimePlatform.WebGLPlayer) {
				gameObject.SetActive(_visibilityForMobile);
			}
		}
	}
}
