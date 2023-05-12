using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FG {
    [RequireComponent(typeof(Movement))]
    public class PlayerController : MonoBehaviour {
        [SerializeField] private SnapOrbitCamera _camera;

        private Movement _movement;
        private Vector2 _pressedPosition;
        private float _pressedPositionTime;

        private bool _moveRoutineIsRunning;
        private bool _isZooming;
        private Coroutine _zoomRoutine;

        public void OnZoom(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }
            
            Vector2 value = context.ReadValue<Vector2>();

            if (context.control.device == Keyboard.current) {
                if (_isZooming) {
                    _isZooming = false;
                    StopCoroutine(_zoomRoutine);
                }
                else {
                    _zoomRoutine = StartCoroutine(PerformZoom(value.y));
                }
            }
            else {
                _camera.Zoom(value.y);
            }
        }

        public void OnLookDrag(InputAction.CallbackContext context) {
            if (!context.performed || (GameManager.Instance.CurrentGameState != GameManager.GameState.CanStart &&
                                       GameManager.Instance.CurrentGameState != GameManager.GameState.Running) ||
                _camera.IsSnappingPosition || _camera.IsSnappingRotation || _movement.IsProcessing ||
                !ReferenceEquals(EventSystem.current.currentSelectedGameObject, null)) {
                return;
            }

            Vector2 position = Vector2.zero;
            if (context.control.device == Touchscreen.current) {
                position = Touchscreen.current.position.ReadValue();
            }

            float direction = position.x > Screen.width * 0.5f ? 1f : -1f;
            StartCoroutine(PerformLook(direction * GameSettings.Instance.CameraInvertLook));
        }

        public void OnMoveDrag(InputAction.CallbackContext context) {
            if (!context.performed || (GameManager.Instance.CurrentGameState != GameManager.GameState.CanStart &&
                                       GameManager.Instance.CurrentGameState != GameManager.GameState.Running) ||
                _camera.IsSnappingPosition || _camera.IsSnappingRotation || _movement.IsProcessing ||
                !ReferenceEquals(EventSystem.current.currentSelectedGameObject, null)) {
                return;
            }

            if (GameManager.Instance.CurrentGameState == GameManager.GameState.CanStart) {
                GameManager.Instance.CurrentGameState = GameManager.GameState.Running;
            }

            Vector2 releasedPosition = Vector2.zero;
            float value = context.ReadValue<float>();
            if ((value + Mathf.Epsilon) >= 1f) {
                // Pressed
                if (context.control.device == Mouse.current) {
                    _pressedPosition = Mouse.current.position.ReadValue();
                }
                else if (context.control.device == Touchscreen.current) {
                    _pressedPosition = Touchscreen.current.position.ReadValue();
                }

                _pressedPositionTime = Time.time;
            }
            else {
                // released
                if (context.control.device == Mouse.current) {
                    releasedPosition = Mouse.current.position.ReadValue();
                }
                else if (context.control.device == Touchscreen.current) {
                    releasedPosition = Touchscreen.current.position.ReadValue();
                }
                else {
                    return;
                }

                if (Vector2.Distance(_pressedPosition, releasedPosition) >=
                    GameSettings.Instance.MinimumSwipeDistance &&
                    Time.time - _pressedPositionTime <= GameSettings.Instance.MaximumSwipeTime) {
                    Vector2 swipeDirection = _pressedPosition.DirectionTo(releasedPosition).normalized;
                    Vector3 moveDirection = Vector3.zero;

                    if (Vector2.Dot(Vector2.up, swipeDirection) > GameSettings.Instance.SwipeDirectionThreshold) {
                        moveDirection = Vector3.forward;
                    }
                    else if (Vector2.Dot(Vector2.down, swipeDirection) >
                             GameSettings.Instance.SwipeDirectionThreshold) {
                        moveDirection = Vector3.back;
                    }
                    else if (Vector2.Dot(Vector2.left, swipeDirection) >
                             GameSettings.Instance.SwipeDirectionThreshold) {
                        moveDirection = Vector3.left;
                    }
                    else if (Vector2.Dot(Vector2.right, swipeDirection) >
                             GameSettings.Instance.SwipeDirectionThreshold) {
                        moveDirection = Vector3.right;
                    }

                    StartCoroutine(PerformMove(moveDirection));
                }
            }
        }
        
        public void OnMove(InputAction.CallbackContext context) {
            if (context.canceled) {
                _moveRoutineIsRunning = false;
            }
            
            if (!context.performed || (GameManager.Instance.CurrentGameState != GameManager.GameState.CanStart &&
                                                            GameManager.Instance.CurrentGameState != GameManager.GameState.Running) ||
                _camera.IsSnappingPosition || _camera.IsSnappingRotation ||
                !ReferenceEquals(EventSystem.current.currentSelectedGameObject, null)) {
                _moveRoutineIsRunning = false;
                return;
            }

            if (GameManager.Instance.CurrentGameState == GameManager.GameState.CanStart) {
                GameManager.Instance.CurrentGameState = GameManager.GameState.Running;
            }

            Vector2 value = context.ReadValue<Vector2>();
            if (_moveRoutineIsRunning || value == Vector2.zero) {
                _moveRoutineIsRunning = false;
            }
            else if (!Mathf.Approximately(Mathf.Abs(value.x), Mathf.Abs(value.y)) && !_movement.IsProcessing) {
                _moveRoutineIsRunning = true;
                StartCoroutine(PerformMove(new Vector3(value.x, 0f, value.y)));
            }
        }

        public void OnLook(InputAction.CallbackContext context) {
            if (!context.performed ||
                (GameManager.Instance.CurrentGameState != GameManager.GameState.CanStart &&
                 GameManager.Instance.CurrentGameState != GameManager.GameState.Running) ||
                _camera.IsSnappingPosition || _camera.IsSnappingRotation || _movement.IsProcessing ||
                !ReferenceEquals(EventSystem.current.currentSelectedGameObject, null)) {
                return;
            }

            StartCoroutine(PerformLook(context.ReadValue<float>() * GameSettings.Instance.CameraInvertLook));
        }

        private IEnumerator PerformMove(Vector3 direction) {
            do {
                _movement.Move(_camera.GetRelativeDirection(direction));
                yield return new WaitUntil(() => !_movement.IsProcessing);
                if (_movement.DidMove) {
                    _camera.SnapPosition();
                }
            } while (_moveRoutineIsRunning);
        }

        private IEnumerator PerformLook(float direction) {
            _camera.SnapRotation(Mathf.RoundToInt(direction));
            yield return new WaitUntil(() => !_camera.IsSnappingRotation && !_camera.IsSnappingPosition);
            _movement.ReorderPlayerBlocks();
        }
        
        private IEnumerator PerformZoom(float direction) {
            _isZooming = true;
            
            while (true) {
                _camera.Zoom(direction * Time.deltaTime * GameSettings.Instance.CameraKeyboardZoomSpeed);
                yield return null;
            }
        }

        private void Awake() {
            _movement = GetComponent<Movement>();
        }
    }
}