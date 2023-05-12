using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FG.Gridmap {
    public class GridMap : MonoBehaviour {
        [SerializeField] private GameObject[] _blockPrefabs = default;

        private Dictionary<Vector3Int, Block> _map = new Dictionary<Vector3Int, Block>();
        
        #if UNITY_EDITOR
        public Dictionary<Vector3Int, Block> Map => _map;
        #endif

        private Transform _transform;

        public GameObject GetBlockPrefab(int index) {
            return _blockPrefabs[index];
        }

        public int GetBlockCount() => _map.Count;

        public bool GetBlock(Vector3Int position, out Block block) {
            if (_map.ContainsKey(position)) {
                block = _map[position];
                return true;
            }

            block = null;
            return false;
        }

        public void GetBlocks(in Vector3Int[] positions, out Block[] blocks) {
            blocks = new Block[positions.Length];
            for (int i = 0; i < positions.Length; i++) {
                GetBlock(positions[i], out blocks[i]);
            }
        }

        public void GetAllBlocks(out List<Block> blocks) {
            blocks = _map.Values.ToList();
        }

        public bool IsOccupied(Vector3Int position) {
            return _map.ContainsKey(position);
        }

        public void AddBlock(Vector3Int position, Block block) {
            block.transform.SetParent(_transform);
            _map.Add(position, block);
        }

        public bool CreateBlock(int prefabIndex, Vector3Int position, bool deleteIfBlock = false) {
            return CreateBlock(prefabIndex, position, out Block block, deleteIfBlock);
        }

        public bool CreateBlock(int prefabIndex, Vector3Int position, out Block block, bool deleteIfBlock = false) {
            if ((prefabIndex < 0 && prefabIndex >= _blockPrefabs.Length - 1) || position.Equals(Vector3Int.zero)) {
                block = null;
                return false;
            }

            if (_map.ContainsKey(position)) {
                if (deleteIfBlock) {
                    Destroy(_map[position].gameObject);
                    _map.Remove(position);
                }
                else {
                    block = null;
                    return false;
                }
            }

            GridMapUtility.RandomSnapRotation(out Quaternion rotation);
            block = Instantiate(_blockPrefabs[prefabIndex], position, rotation, _transform)
                ?.GetComponent<Block>();

            block.PrefabIndex = prefabIndex;
            _map.Add(position, block);

            if (!GameSettings.Instance.ShowBlockSymbols) {
                MeshRenderer renderer = block.GetComponent<MeshRenderer>();
                if (!ReferenceEquals(renderer, null) && renderer.materials.Length > 1) {
                    Material[] materials = renderer.materials;
                    Material[] newMaterials = new[] {materials[0]};
                    renderer.materials = newMaterials;
                }
            }

            return true;
        }

        public bool RemoveBlock(Vector3Int position, bool destroyTile = false, float destroyTileDelay = 0f) {
            if (_map.TryGetValue(position, out Block block)) {
                if (destroyTile) {
                    Destroy(block.gameObject, destroyTileDelay);
                }

                _map.Remove(position);
                return true;
            }

            return false;
        }

        public void CreateStartBlock() {
            GridMapUtility.RandomSnapRotation(out Quaternion rotation);
            PlayerBlock block = Instantiate(_blockPrefabs[1], Vector3Int.zero, rotation, _transform)
                ?.GetComponent<PlayerBlock>();

            block.IsActive = true;
            _map.Add(Vector3Int.zero, block);

            if (!GameSettings.Instance.ShowBlockSymbols) {
                MeshRenderer renderer = block.GetComponent<MeshRenderer>();
                if (!ReferenceEquals(renderer, null) && renderer.materials.Length > 1) {
                    Material[] materials = renderer.materials;
                    Material[] newMaterials = new[] {materials[0]};
                    renderer.materials = newMaterials;
                }
            }
        }

        public void ClearMap() {
            foreach (Transform child in _transform) {
                Destroy(child.gameObject);
            }

            _map.Clear();
        }

        private void Awake() {
            _transform = transform;
        }
    }
}