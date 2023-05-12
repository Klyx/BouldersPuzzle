using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FG {
    public class FixScrollRectDragChildren : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
        IScrollHandler {
        private ScrollRect _ScrollRect;

        public void OnBeginDrag(PointerEventData eventData) {
            _ScrollRect.OnBeginDrag(eventData);
        }


        public void OnDrag(PointerEventData eventData) {
            _ScrollRect.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData) {
            _ScrollRect.OnEndDrag(eventData);
        }


        public void OnScroll(PointerEventData data) {
            _ScrollRect.OnScroll(data);
        }

        private void Awake() {
            _ScrollRect = transform.GetComponentInParent<ScrollRect>();
        }
    }
}