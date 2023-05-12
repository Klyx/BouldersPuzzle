using System.Globalization;
using FG.Gridmap;
using UnityEngine;
using TMPro;

namespace FG {
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class BlockCount : MonoBehaviour {
		private TextMeshProUGUI _blockCount;

		public void SetBlockCount(int count) {
			if (!_blockCount.text.Equals(count.ToString(CultureInfo.InvariantCulture))) {
				_blockCount.text = count.ToString(CultureInfo.InvariantCulture);
			}
		}
		
		public void SetBlockCount(GridMap map, Block block, bool added, Vector3Int position) {
			_blockCount.text = map.GetBlockCount().ToString(CultureInfo.InvariantCulture);
		}
		
		private void Awake() {
			_blockCount = GetComponent<TextMeshProUGUI>();
		}
	}
}
