using UnityEngine;
using UnityEngine.Audio;

namespace FG {
	[DefaultExecutionOrder(-100)]
	public class AudioManager : MonoBehaviour {
		public static AudioManager Instance { get; private set; }
		
		[SerializeField] private AudioMixer _audioMixer;

		private AudioSource _duplicator;
		private AudioSource _destroyer;

		public void PlayDuplicatorSound(Vector3Int position) {
			_duplicator.transform.position = position;
			_duplicator.Play();
		}
		
		public void PlayDestroyerSound(Vector3Int position) {
			_destroyer.transform.position = position;
			_destroyer.Play();
		}

		public void SetVolume() {
			_audioMixer.SetFloat("Master_Volume", Mathf.Log10(GameSettings.Instance.MasterVolume) * 20f);
			_audioMixer.SetFloat("Music_Volume", Mathf.Log10(GameSettings.Instance.MusicVolume) * 20f);
			_audioMixer.SetFloat("SFX_Volume", Mathf.Log10(GameSettings.Instance.SFXVolume) * 20f);
			_audioMixer.SetFloat("UI_Volume", Mathf.Log10(GameSettings.Instance.UIVolume) * 20f);
		}
		
		private void Awake() {
			if (Instance) {
				Destroy(gameObject);
				return;
			}

			Transform sfxTransform = transform.Find("SFX");
			_duplicator = sfxTransform.GetChild(0).GetComponent<AudioSource>();
			_destroyer = sfxTransform.GetChild(1).GetComponent<AudioSource>();

			Instance = this;
		}
	}
}
