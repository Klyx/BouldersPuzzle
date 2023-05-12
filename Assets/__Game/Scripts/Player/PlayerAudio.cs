using UnityEngine;

namespace FG {
	[RequireComponent(typeof(AudioSource))]
	public class PlayerAudio : MonoBehaviour {
		[SerializeField] private AudioClip _moveAudio;
		[SerializeField] private AudioClip _cantMoveAudio;
		
		private AudioSource _audio;
		
		public void OnMove() {
			_audio.PlayOneShot(_moveAudio);
		}

		public void OnCantMove() {
			if (!_audio.isPlaying) {
				_audio.PlayOneShot(_cantMoveAudio);
			}
		}

		private void Awake() {
			_audio = GetComponent<AudioSource>();
		}
	}
}
