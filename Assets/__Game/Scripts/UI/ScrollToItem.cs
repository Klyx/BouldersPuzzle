using UnityEngine;
using UnityEngine.UI;

namespace FG {
    public class ScrollToItem : MonoBehaviour {
        private ScrollRect _scrollRect;

        public void OnRadioGroupChanged(int index) {
            float verticalPosition = 1f - (float)index / (_scrollRect.content.childCount - 1);
            _scrollRect.verticalNormalizedPosition = verticalPosition;

        }

        private void Awake() {
            _scrollRect = GetComponent<ScrollRect>();
        }
    }
}