using System.Collections;
using UnityEngine;

namespace FG {
    public class Shake : MonoBehaviour {
        private Transform _transform;
        private Vector3 _originalPosition;
        private Coroutine _shakeRoutine;

        public bool IsShaking { get; private set; } = false;

        private void Awake() {
            _transform = transform;
            _originalPosition = _transform.localPosition;
        }

        public void StartShake() {
            if (!GameSettings.Instance.ShakeCamera) {
                return;
            }
            
            if (IsShaking) {
                StopCoroutine(_shakeRoutine);
            }
            
            _shakeRoutine = StartCoroutine(PerformShake(GameSettings.Instance.ShakeCameraMoveDuration, GameSettings.Instance.ShakeCameraMoveAmount));
        }
        
        public void StartSmallShake() {
            if (!GameSettings.Instance.ShakeCamera) {
                return;
            }
            
            if (IsShaking) {
                StopCoroutine(_shakeRoutine);
            }
            
            _shakeRoutine = StartCoroutine(PerformShake(GameSettings.Instance.ShakeCameraCantMoveDuration, GameSettings.Instance.ShakeCameraCantMoveAmount));
        }

        private IEnumerator PerformShake(float shakeDuration, float shakeAmount) {
            IsShaking = true;

            float elapsedTime = 0f;
            while (elapsedTime < shakeDuration) {
                _transform.localPosition = _originalPosition + Random.insideUnitSphere * shakeAmount;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _transform.localPosition = _originalPosition;

            IsShaking = false;
        }
    }
}