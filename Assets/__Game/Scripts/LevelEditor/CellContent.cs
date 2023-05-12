using FG.Gridmap;
using TMPro;
using UnityEngine;

namespace FG {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CellContent : MonoBehaviour {
        private TextMeshProUGUI _cellContent;

        public void SetCellContent(Block block) {
            if (ReferenceEquals(block, null)) {
                Localization.Instance.GetText("Empty", out var emptyText);
                
                if (!_cellContent.text.Equals(emptyText)) {
                    _cellContent.text = emptyText;
                }
            }
            else if (!_cellContent.text.Equals(block.BlockName) &&
                Localization.Instance.GetText(block.BlockName, out string text)) {
                _cellContent.text = text;
            }
        }

        private void Awake() {
            _cellContent = GetComponent<TextMeshProUGUI>();
            Localization.Instance.GetText("Empty", out string emptyText);
            _cellContent.text = emptyText;
        }
    }
}