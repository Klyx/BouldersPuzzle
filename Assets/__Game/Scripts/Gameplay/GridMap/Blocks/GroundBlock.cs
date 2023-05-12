using UnityEngine;

namespace FG.Gridmap {
	public class GroundBlock : Block {
		public override string BlockName { get; set; } = "Ground";
		public override int Priority { get; protected set; } = 0;
		public override bool DisruptConnection { get; protected set; } = false;
		
		public override bool ConnectWithPlayer(PlayerBlock playerBlock, GridMap gridMap) {
			return false;
		}
	}
}
