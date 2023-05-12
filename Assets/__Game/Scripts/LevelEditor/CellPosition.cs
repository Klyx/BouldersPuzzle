using System.Globalization;
using TMPro;
using UnityEngine;

namespace FG.LevelEditor {
	public class CellPosition : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI _cellValueX;
		[SerializeField] private TextMeshProUGUI _cellValueY;
		[SerializeField] private TextMeshProUGUI _cellValueZ;

		public void SetPositions(Vector3Int position) {
			SetX(position.x);
			SetY(position.y);
			SetZ(position.z);
		}

		public void SetX(int x) {
			_cellValueX.text = x.ToString(CultureInfo.InvariantCulture);
		}
		
		public void SetY(int y) {
			_cellValueY.text = y.ToString(CultureInfo.InvariantCulture);
		}
		
		public void SetZ(int z) {
			_cellValueZ.text = z.ToString(CultureInfo.InvariantCulture);
		}
	}
}
