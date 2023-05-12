using System;
using UnityEngine;

namespace FG {
    public class EditorCamera : MonoBehaviour {
        [SerializeField] private float _lookDegree = 45f;
        [SerializeField] private float _distance = 20f;

        private Transform _transform;
        private Camera _camera;
        [NonSerialized] public Vector3 targetPosition = Vector3.zero;

        public void Move(Vector3 moveDirection) {
            Quaternion lookRotation = Quaternion.Euler(_lookDegree, _transform.eulerAngles.y, 0f);
            Vector3 lookDirection = lookRotation * Vector3.forward;

            moveDirection = Vector3.ProjectOnPlane(_transform.TransformDirection(moveDirection), Vector3.up).normalized;
            
            targetPosition += moveDirection;
            Vector3 endPoint = targetPosition - lookDirection * _distance;
            _transform.SetPositionAndRotation(endPoint, lookRotation);
        }

        public void Rotate(float direction) {
            Quaternion lookRotation = Quaternion.Euler(_lookDegree,
                _transform.eulerAngles.y + (90f * direction), 0f);
            Vector3 lookDirection = lookRotation * Vector3.forward;
            Vector3 lookPosition = targetPosition - lookDirection * _distance;
            transform.SetPositionAndRotation(lookPosition, lookRotation);
        }

        public Ray ScreenPointToRay(Vector3 position) {
            return _camera.ScreenPointToRay(position);
        }

        private void Start() {
            Quaternion lookRotation = Quaternion.Euler(_lookDegree, 0f, 0f);
            Vector3 lookDirection = lookRotation * Vector3.forward;
            Vector3 lookPosition = targetPosition - lookDirection * _distance;
            transform.SetPositionAndRotation(lookPosition, lookRotation);
        }

        private void Awake() {
            _transform = transform;
            _camera = GetComponent<Camera>();
        }
    }
}