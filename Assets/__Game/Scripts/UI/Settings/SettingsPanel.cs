using UnityEngine;

namespace FG {
	public class SettingsPanel : MonoBehaviour {
		private int _previousIndex;

		private Transform _transform;
		
		public void OnIndexChange(int index) {
			_transform.GetChild(_previousIndex).gameObject.SetActive(false);
			_transform.GetChild(index).gameObject.SetActive(true);
			_previousIndex = index;
		}

		private void Awake() {
			_transform = transform;
		}
	}
}
