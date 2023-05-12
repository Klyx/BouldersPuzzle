using System;
using FG.Gridmap;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FG.LevelEditor {
    [RequireComponent(typeof(EditorUndo))]
    public class EditorController : MonoBehaviour {
        [SerializeField] private EditorCamera _editorCamera;
        [SerializeField] private GridMap _map;
        [SerializeField] private RadioGroup _blocksGroup;
        [SerializeField] private Transform _selectionObject;
        [SerializeField] private UnityEvent<Vector3Int> _selectedCellPosition;
        [SerializeField] private UnityEvent<Block> _selectedCellBlock;

        [SerializeField, Tooltip("True if added, else removed")]
        private UnityEvent<GridMap, Block, bool, Vector3Int> _onBlockPlaced;

        private Camera _camera;

        private Single _alternativeButtonInput;
        private Vector3 _moveCameraInput;
        private float _moveCameraUp;
        private Vector3 _rotateCameraInput;

        private Vector3Int _currentCellPosition;
        private float _selectionObjectDistance = 14f;
        private Plane _plane = new Plane(Vector3.up, 0f);
        private Vector3 _mousePosition;
        private bool _isPointerOverGameObject;

        private Block _blockInSelectedCell;
        private int _selectedBlockIndex;

        private EditorUndo _undo;

        private Vector3 _startPosition;
        private Quaternion _startOrientation;

        public Vector3Int CurrentCellPosition {
            get => _currentCellPosition;
            set
            {
                if (!Mathf.Approximately(_alternativeButtonInput, 0f) || _currentCellPosition == value) {
                    return;
                }

                _currentCellPosition = value;
                _map.GetBlock(_currentCellPosition, out _blockInSelectedCell);
                _selectionObject.transform.position = value;
                _selectedCellPosition.Invoke(_currentCellPosition);
                _selectedCellBlock.Invoke(_blockInSelectedCell);
            }
        }

        public void PerformUndo() {
            if (_undo.MostRecentUndoPosition(out Vector3Int undoPosition)) {
                if (_map.GetBlock(undoPosition, out Block previousBlock)) {
                    _onBlockPlaced.Invoke(_map, previousBlock, false, undoPosition);
                }
            }

            if (_undo.PerformUndo(_map, out bool added, out Vector3Int position)) {
                _map.GetBlock(position, out Block block);
                _onBlockPlaced.Invoke(_map, block, added, position);
            }
        }

        public void Undo(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }

            PerformUndo();
        }

        public void GoHome(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }
            
            _camera.transform.position = _startPosition;
            _camera.transform.rotation = _startOrientation;
        }

        public void OnCurrentBlockIndexChanged(int index) {
            _selectedBlockIndex = index;
        }

        public void PlaceBlock(InputAction.CallbackContext context) {
            if (!context.performed || _isPointerOverGameObject || _currentCellPosition == Vector3Int.zero) {
                return;
            }

            _map.GetBlock(_currentCellPosition, out Block previousBlock);

            if (!ReferenceEquals(previousBlock, null) && previousBlock.PrefabIndex == _selectedBlockIndex) {
                return;
            }

            if (!ReferenceEquals(previousBlock, null)) {
                _onBlockPlaced.Invoke(_map, previousBlock, false, _currentCellPosition);
            }

            _undo.AddUndo(ReferenceEquals(previousBlock, null) ? -1 : previousBlock.PrefabIndex, _currentCellPosition);

            _map.CreateBlock(_selectedBlockIndex, _currentCellPosition, out Block block, true);
            _onBlockPlaced.Invoke(_map, block, true, _currentCellPosition);
        }

        public void RemoveBlock(InputAction.CallbackContext context) {
            if (!context.performed || _isPointerOverGameObject || _currentCellPosition == Vector3Int.zero) {
                return;
            }

            if (_map.GetBlock(_currentCellPosition, out Block block)) {
                _onBlockPlaced.Invoke(_map, block, false, _currentCellPosition);
                _undo.AddUndo(block.PrefabIndex, _currentCellPosition);
                _map.RemoveBlock(_currentCellPosition, true);
            }
        }

        public void OnScroll(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }

            Vector2 value = context.ReadValue<Vector2>();

            if (Mathf.Approximately(_alternativeButtonInput, 0)) {
                _selectionObjectDistance = Mathf.Clamp(_selectionObjectDistance + value.y,
                    GameSettings.Instance.EditorMinSelectionDistance, GameSettings.Instance.EditorMaxSelectionDistance);

                GetCellPosition(_mousePosition);
            }
            else {
                _blocksGroup.Step(Mathf.RoundToInt(value.y));
            }
        }

        public void OnAlternativeButton(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }

            _alternativeButtonInput = context.ReadValue<Single>();
            Cursor.lockState = CursorLockMode.Confined;
            if (Mathf.Approximately(_alternativeButtonInput, 0f)) {
                Cursor.lockState = CursorLockMode.None;
                _selectionObject.gameObject.SetActive(true);
            }
            else {
                Cursor.lockState = CursorLockMode.Locked;
                _selectionObject.gameObject.SetActive(false);
            }
        }

        public void OnMouseMove(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }

            _mousePosition = ((Vector3)context.ReadValue<Vector2>()).ToVector3Int();
            GetCellPosition(_mousePosition);
        }

        public void OnRotateCamera(InputAction.CallbackContext context) {
            if (Mathf.Approximately(_alternativeButtonInput, 0f) || !context.performed ||
                EventSystem.current.currentSelectedGameObject) {
                return;
            }

            Vector2 value = context.ReadValue<Vector2>();
            _rotateCameraInput.Set(value.y * GameSettings.Instance.CameraInvertLook, value.x, 0f);
            _editorCamera.Rotate(_rotateCameraInput);
        }

        public void OnMoveVertical(InputAction.CallbackContext context) {
            if (!context.performed || EventSystem.current.currentSelectedGameObject) {
                return;
            }

            _moveCameraInput.z = context.ReadValue<float>();
        }

        public void OnMoveHorizontal(InputAction.CallbackContext context) {
            if (!context.performed || EventSystem.current.currentSelectedGameObject) {
                return;
            }

            _moveCameraInput.x = context.ReadValue<float>();
        }

        public void OnMoveUp(InputAction.CallbackContext context) {
            if (!context.performed || EventSystem.current.currentSelectedGameObject) {
                return;
            }

            _moveCameraUp = context.ReadValue<float>();
        }

        private void GetCellPosition(Vector3 value) {
            Ray ray = _camera.ScreenPointToRay(value);
            _plane.SetNormalAndPosition(Vector3.up,
                _editorCamera.transform.position + _editorCamera.transform.forward * _selectionObjectDistance);
            if (_plane.Raycast(ray, out var enter)) {
                CurrentCellPosition = ray.GetPoint(enter).ToVector3Int();
            }
        }

        private void Update() {
            _editorCamera.Move(_moveCameraInput, _moveCameraUp);
            GetCellPosition(_mousePosition);
            _isPointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
            _selectionObject.gameObject.SetActive(!_isPointerOverGameObject);
        }

        private void Awake() {
            _camera = Camera.main;
            _plane.SetNormalAndPosition(Vector3.up,
                _editorCamera.transform.position + _editorCamera.transform.forward * _selectionObjectDistance);

            _undo = GetComponent<EditorUndo>();

            _startPosition = _camera.transform.position;
            _startOrientation = _camera.transform.rotation;
        }
    }
}