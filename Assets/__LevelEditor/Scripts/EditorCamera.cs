using UnityEngine;

namespace FG.LevelEditor {
    public class EditorCamera : MonoBehaviour {
        private Transform _transform;

        public void Move(Vector3 moveInput, float upInput) {
            float z = moveInput.z;
            moveInput.z = 0f;
            _transform.Translate(moveInput * GameSettings.Instance.EditorCameraMoveSpeed * Time.deltaTime, Space.Self);
            moveInput.Set(0f, 0f, z);
            _transform.Translate(moveInput * GameSettings.Instance.EditorCameraMoveSpeed * Time.deltaTime, Space.World);
            if (!Mathf.Approximately(upInput, 0f)) {
                _transform.Translate(
                    Vector3.up * upInput * GameSettings.Instance.EditorCameraMoveSpeed * Time.deltaTime, Space.World);
            }
        }

        public void Rotate(Vector3 rotateInput) {
            _transform.eulerAngles += rotateInput * Time.deltaTime * GameSettings.Instance.EditorCameraRotateSpeed;
        }

        private void Awake() {
            _transform = transform;
        }
    }
}