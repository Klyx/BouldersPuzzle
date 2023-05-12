using FG.Gridmap;
using FG.LevelEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FG {
    public class PlayerEditorController : MonoBehaviour {
        [SerializeField] private EditorCamera _camera;
        [SerializeField] private GridMap _gridMap;
        [SerializeField] private CellPosition _statusbarCellPosition;
        [SerializeField] private CellContent _statusbarCellContent;
        [SerializeField] private BlockCount _statusbarBlockCount;
        [SerializeField] private Transform _selectionObject;
        [SerializeField] private RadioGroup _blocksGroup;

        [Header("Events")] [Tooltip("True on saved")]
        public BoolEvent onLevelSavedStatusChanged = new BoolEvent();

        private Plane _plane = new Plane(Vector3.up, 0f);
        private int _yLevel = 0;

        private Vector3Int _currentCell;

        public int CurrentBlockIndex { get; set; }

        public Vector3Int CurrentCell {
            get { return _currentCell; }
            set
            {
                bool positionIsUpdated = false;
                if (!value.x.Equals(_currentCell.x)) {
                    _statusbarCellPosition.SetX(value.x);
                    positionIsUpdated = true;
                }

                if (!value.y.Equals(_currentCell.y)) {
                    _statusbarCellPosition.SetY(value.y);
                    positionIsUpdated = true;
                }

                if (!value.z.Equals(_currentCell.z)) {
                    _statusbarCellPosition.SetZ(value.z);
                    positionIsUpdated = true;
                }

                if (positionIsUpdated) {
                    _selectionObject.transform.position = value;
                    _gridMap.GetBlock(value, out Block block);
                    _statusbarCellContent.SetCellContent(block);
                }

                _currentCell = value;
            }
        }

        public void OnSaved() {
            onLevelSavedStatusChanged.Invoke(true);
        }

        public void OnPlaceBlock(InputAction.CallbackContext context) {
            if (!context.performed || EventSystem.current.IsPointerOverGameObject() ||
                !ReferenceEquals(EventSystem.current.currentSelectedGameObject, null)) {
                return;
            }

            bool hadBlock = _gridMap.IsOccupied(_currentCell);
            if (_gridMap.CreateBlock(CurrentBlockIndex, _currentCell, out Block block, true)) {
                _statusbarCellContent.SetCellContent(block);
                if (!hadBlock) {
                    _statusbarBlockCount.SetBlockCount(_gridMap.transform.childCount);
                }

                onLevelSavedStatusChanged.Invoke(false);
            }
        }

        public void OnRemoveBlock(InputAction.CallbackContext context) {
            if (!context.performed || EventSystem.current.IsPointerOverGameObject() ||
                !ReferenceEquals(EventSystem.current.currentSelectedGameObject, null)) {
                return;
            }

            if (_gridMap.RemoveBlock(_currentCell, true)) {
                _statusbarCellContent.SetCellContent(null);
                // - 1 because the object isn't destroyed yet.
                _statusbarBlockCount.SetBlockCount(_gridMap.transform.childCount - 1);
                onLevelSavedStatusChanged.Invoke(false);
            }
        }

        public void OnScrollWheel(InputAction.CallbackContext context) {
            if (!context.performed) {
                return;
            }

            Vector2 value = context.ReadValue<Vector2>();
            _blocksGroup.Step((int) value.y);
        }

        public void OnStepY(InputAction.CallbackContext context) {
            if (!context.performed || EventSystem.current.currentSelectedGameObject) {
                return;
            }

            float value = context.ReadValue<float>();
            if (value < 0f) {
                _yLevel--;
                _camera.Move(Vector3.down);
            }
            else if (value > 0f) {
                _yLevel++;
                _camera.Move(Vector3.up);
            }
            
            _plane.SetNormalAndPosition(Vector3.up, new Vector3(0f, _yLevel, 0f));
            Vector3Int cellPosition = CurrentCell;
            cellPosition.y = _yLevel;
            CurrentCell = cellPosition;
        }

        public void OnMouseMove(InputAction.CallbackContext context) {
            if (!_camera || !_selectionObject) {
                return;
            }

            Vector3Int value = ((Vector3) context.ReadValue<Vector2>()).ToVector3Int();

            Ray ray = _camera.ScreenPointToRay(value);
            float enter;
            if (_plane.Raycast(ray, out enter)) {
                Vector3Int newPosition = ray.GetPoint(enter).ToGridPosition();
                newPosition.y = _yLevel;
                CurrentCell = newPosition;
            }
        }

        public void OnMove(InputAction.CallbackContext context) {
            if (!context.performed || EventSystem.current.currentSelectedGameObject) {
                return;
            }
            
            Vector2 value = context.ReadValue<Vector2>();
            _camera.Move(new Vector3(value.x, 0f, value.y));
        }
        
        public void OnLook(InputAction.CallbackContext context) {
            if (!context.performed || EventSystem.current.currentSelectedGameObject) {
                return;
            }
            
            _camera.Rotate(context.ReadValue<float>() * GameSettings.Instance.CameraInvertLook);
        }

        private void Update() {
            _selectionObject.gameObject.SetActive(!EventSystem.current.IsPointerOverGameObject());
        }
    }
}