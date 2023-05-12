using System;
using System.Collections;
using System.Collections.Generic;
using FG.Gridmap;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace FG {
    public class Movement : MonoBehaviour {
        [SerializeField] private GridMap _gridMap;
        [SerializeField] private Transform _cameraTransform;
        
        [Header("Events")]
        public UnityEvent onMoveEvent;
        public UnityEvent onMoveFailedEvent;
        public UnityEvent afterConnectEvent;

        private Transform _transform;
        [NonSerialized] public List<PlayerBlock> _playerBlocks = new List<PlayerBlock>(10);

        public bool IsProcessing { get; private set; } = false;
        public bool DidMove { get; private set; } = false;

        public void Move(Vector3Int moveDirection) {
            StartCoroutine(PerformMove(moveDirection));
        }

        private IEnumerator PerformMove(Vector3Int moveDirection) {
            IsProcessing = true;
            DidMove = false;

            if (!MovementUtility.UpdateMovablePlayerBlocks(moveDirection, _gridMap, ref _playerBlocks)) {
                IsProcessing = false;
                onMoveFailedEvent.Invoke();
#if UNITY_IPHONE || UNITY_ANDROID
                if (Application.isMobilePlatform) {
                    Handheld.Vibrate();
                }
#endif
                yield break;
            }

            for (int i = 0; i < _playerBlocks.Count; i++) {
                _gridMap.RemoveBlock(_playerBlocks[i].PositionInt);
                _playerBlocks[i].transform.SetParent(_transform);
                MovementUtility.DisconnectPlayerFromGoalBlocks(i, _playerBlocks[i], _gridMap);
            }

            MovementUtility.GetTotalRenderBounds(ref _playerBlocks, out Bounds bounds);
            MovementUtility.ReorderPlayerBlocks(ref _playerBlocks, _transform, _cameraTransform, bounds);
            Vector3 rotationPoint = MovementUtility.GetRotationPoint(moveDirection, in bounds);
            Vector3 axis = Vector3.Cross(Vector3.up, moveDirection.ToVector3()).normalized;
            yield return null;

            GameManager.Instance.Steps++;
            GameManager.Instance.Moves += _playerBlocks.Count;

            #region Move

            float step = 0f;
            float rate = 1f / GameSettings.Instance.BlockRotationTime;
            float lastStep = 0f;

            while (step < 1.0f) {
                step += Time.deltaTime * rate;
                float smoothStep = Mathf.SmoothStep(0f, 1f, step);
                _transform.RotateAround(rotationPoint, axis, 90f * (smoothStep - lastStep));
                lastStep = smoothStep;

                yield return null;
            }

            if (step > 1f) {
                _transform.RotateAround(rotationPoint, axis, 90f * (1f - lastStep));
            }
            
            foreach (Block block in _playerBlocks) {
                block.transform.position = block.PositionInt;
            }

            onMoveEvent.Invoke();
            DidMove = true;
            #endregion Move
            
            MovementUtility.ConnectWithBlocks(_cameraTransform, ref _playerBlocks, _gridMap);
            afterConnectEvent.Invoke();

            yield return null;
            MovementUtility.GetTotalRenderBounds(ref _playerBlocks, out bounds);
            MovementUtility.ReorderPlayerBlocks(ref _playerBlocks, _transform, _cameraTransform, bounds);
            
            if (_playerBlocks.Count == 0 || !MovementUtility.StandsOnValidBlock(ref _playerBlocks, _gridMap)) {
                yield return new WaitForSeconds(GameSettings.Instance.WaitBeforeLoose);
                GameManager.Instance.CurrentGameState = GameManager.GameState.GameOver;
                IsProcessing = false;
            }

            foreach (Block block in _playerBlocks) {
                _gridMap.AddBlock(block.PositionInt, block);
            }

            IsProcessing = false;
        }

        public void ReorderPlayerBlocks() {
            MovementUtility.GetTotalRenderBounds(ref _playerBlocks, out Bounds bounds);
            MovementUtility.ReorderPlayerBlocks(ref _playerBlocks, _transform, _cameraTransform, bounds);
        }

        private void OnGameStateChanged(GameManager.GameState gameState) {
            if (gameState == GameManager.GameState.CanStart) {
                _gridMap.GetBlock(Vector3Int.zero, out Block block);
                Assert.IsNotNull(block, "block != null");
                Assert.IsNotNull((PlayerBlock)block, "block != PlayerBlock");
                _playerBlocks.Add((PlayerBlock)block);
            }
        }

        private void Awake() {
            _transform = transform;
            GameManager.Instance.gameStateEvent.AddListener(OnGameStateChanged);

#if UNITY_EDITOR
            Selection.activeGameObject = gameObject;
#endif
        }
    }
}