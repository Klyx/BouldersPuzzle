using System.Collections;
using UnityEngine;

namespace FG {
    public class SnapOrbitCamera : MonoBehaviour {
        [SerializeField] private Transform target;

        [SerializeField] private float _lookDegree = 45f;
        [SerializeField] private float _distance = 20f;

        [SerializeField] private AnimationCurve _snappingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField] private float _minCameraZoom = 30f;
        [SerializeField] private float _maxCameraZoom = 90f;

        private Transform _transform;
        private Camera _camera;

        public bool IsSnappingRotation { get; private set; } = false;
        public bool IsSnappingPosition { get; private set; } = false;

        public Vector3Int GetRelativeDirection(Vector3 direction) {
            return Vector3.ProjectOnPlane(_transform.TransformDirection(direction), Vector3.up).normalized
                .ToVector3Int();
        }

        public Vector3Int GetCameraDirection() {
            return Vector3.ProjectOnPlane(_transform.forward, Vector3.up).normalized
                .ToVector3Int();
        }

        public void SnapRotation(int direction) {
            if (!IsSnappingRotation) {
                StartCoroutine(SnapCameraRotation(direction * 90f));
            }
        }

        public void SnapPosition() {
            if (!IsSnappingPosition) {
                StartCoroutine(SnapCameraPosition());
            }
        }

        public void Zoom(float direction) => _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView + direction,
            _minCameraZoom, _maxCameraZoom);

        private IEnumerator SnapCameraPosition() {
            IsSnappingPosition = true;

            Quaternion lookRotation = Quaternion.Euler(_lookDegree, _transform.eulerAngles.y, 0f);
            Vector3 lookDirection = lookRotation * Vector3.forward;
            Vector3 startPoint = _transform.position;

            float progress = 0f;
            float rate = 1f / GameSettings.Instance.CameraSnapToPositionTime;
            Vector3 endPoint = target.position - lookDirection * _distance;
            endPoint = endPoint.ToVector3Int();
            while (progress < 1f) {
                progress += Time.deltaTime * rate;

                Vector3 lookPosition = Vector3.Lerp(startPoint, endPoint, _snappingCurve.Evaluate(progress));
                _transform.SetPositionAndRotation(lookPosition, lookRotation);

                yield return null;
            }

            _transform.position = endPoint;
            IsSnappingPosition = false;
        }

        private IEnumerator SnapCameraRotation(float angle) {
            IsSnappingRotation = true;

            yield return new WaitUntil(() => !IsSnappingPosition);

            float startAngle = _transform.rotation.eulerAngles.y;
            float endAngle = startAngle - angle;

            float progress = 0f;
            float rate = 1f / GameSettings.Instance.CameraSnapToRotationTime;
            while (progress < 1f) {
                progress += Time.deltaTime * rate;

                Quaternion lookRotation = Quaternion.Euler(_lookDegree,
                    Mathf.Lerp(startAngle, endAngle, _snappingCurve.Evaluate(progress)), 0f);
                Vector3 lookDirection = lookRotation * Vector3.forward;
                Vector3 lookPosition = target.position - lookDirection * _distance;
                transform.SetPositionAndRotation(lookPosition, lookRotation);

                yield return null;
            }

            IsSnappingRotation = false;
        }

        private void Start() {
            Quaternion lookRotation = Quaternion.Euler(_lookDegree, 0f, 0f);
            Vector3 lookDirection = lookRotation * Vector3.forward;
            Vector3 lookPosition = target.position - lookDirection * _distance;
            transform.SetPositionAndRotation(lookPosition, lookRotation);
        }

        private void Awake() {
            _transform = transform;
            _camera = Camera.main;
        }
    }
}