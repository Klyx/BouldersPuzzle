using UnityEngine;

namespace FG {
	public class DontDestroyObjectOnLoad : MonoBehaviour {
		private void Awake() {
			DontDestroyOnLoad(gameObject);
		}
	}
}
