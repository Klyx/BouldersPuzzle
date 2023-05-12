using System.Collections;
using UnityEngine;

namespace FG {
    [RequireComponent(typeof(AudioSource))]
    public class Jukebox : MonoBehaviour {
        [SerializeField] private AudioClip[] _musicClips = new AudioClip[] {};

        private AudioSource _audioSource;
		
        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start() {
            StartCoroutine(PlayMusic());
        }

        private IEnumerator PlayMusic() {
            while (true) {
                _audioSource.clip = _musicClips[Random.Range(0, _musicClips.Length)];
                _audioSource.Play();
                yield return new WaitForSeconds(_audioSource.clip.length);
            }
        }
    }
}