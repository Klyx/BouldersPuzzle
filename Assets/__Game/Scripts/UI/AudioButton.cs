using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FG {
    [RequireComponent(typeof(Button)), RequireComponent(typeof(AudioSource)), RequireComponent(typeof(EventTrigger))]
    public class AudioButton : MonoBehaviour {
        [SerializeField] private ButtonSettings _buttonSettings;
        
        private AudioSource _audioSource;
        private EventTrigger _eventTrigger;

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
            _eventTrigger = GetComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { OnPointerEnter((PointerEventData) data); });
            _eventTrigger.triggers.Add(entry);
            
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { OnPointerClick((PointerEventData) data); });
            _eventTrigger.triggers.Add(entry);
        }

        private void OnPointerEnter(PointerEventData data) {
            if (_buttonSettings._enterSound) {
                _audioSource.clip = _buttonSettings._enterSound;
                _audioSource.Play();
            }
        }
        
        private void OnPointerClick(PointerEventData data) {
            if (_buttonSettings._clickSound) {
                _audioSource.clip = _buttonSettings._clickSound;
                _audioSource.Play();
            }
        }
    }
}