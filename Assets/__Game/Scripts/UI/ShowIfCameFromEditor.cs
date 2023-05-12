using UnityEngine;

namespace FG {
	public class ShowIfCameFromEditor : MonoBehaviour {
		private void Awake() {
			gameObject.SetActive(GameManager.Instance.CameFromEditor);
		}
	}
}
