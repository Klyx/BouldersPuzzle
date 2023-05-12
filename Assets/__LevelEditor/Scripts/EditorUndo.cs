using System.Collections.Generic;
using FG.Gridmap;
using UnityEngine;
using UnityEngine.UI;

namespace FG {
    public class EditorUndo : MonoBehaviour {
        [SerializeField] private Button _undoButton;
        
        private struct Undo {
            public int previousIndex;
            public Vector3Int position;

            public Undo(int previousIndex, Vector3Int position) {
                this.previousIndex = previousIndex;
                this.position = position;
            }
        }

        private List<Undo> _undos;

        public void ClearUndos() {
            _undos.Clear();
            _undoButton.interactable = false;
        }

        public bool MostRecentUndoPosition(out Vector3Int position) {
            position = Vector3Int.zero;
            if (_undos.Count > 0) {
                position = _undos[^1].position;
                return true;
            }

            return false;
        }

        // True on did undo..
        public bool PerformUndo(GridMap map, out bool added, out Vector3Int position) {
            added = false;
            position = Vector3Int.zero;
            if (_undos.Count < 1) {
                return false;
            }

            Undo undo = _undos[^1];
            position = undo.position;
            _undos.RemoveAt(_undos.Count - 1);
            if (undo.previousIndex < 0) {
                map.RemoveBlock(undo.position, true);
            }
            else {
                added = true;
                map.CreateBlock(undo.previousIndex, undo.position, true);
            }

            if (_undos.Count == 0) {
                _undoButton.interactable = false;
            }

            return true;
        }

        public void AddUndo(int previousIndex, Vector3Int position) {
            if (_undos.Count >= GameSettings.Instance.EditorMaxUndos) {
                _undos.RemoveAt(0);
            }

            _undos.Add(new Undo(previousIndex, position));
            _undoButton.interactable = true;
        }

        private void Awake() {
            _undos = new List<Undo>(GameSettings.Instance.EditorMaxUndos);
        }
    }
}