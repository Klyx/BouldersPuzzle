
namespace FG.Gridmap {
	public class DuplicatorBlock : Block {

		public override string BlockName { get; set; } = "Duplicator";
		public override int Priority { get; protected set; } = 40;
		public override bool DisruptConnection { get; protected set; } = true;

		public override bool ConnectWithPlayer(PlayerBlock playerBlock, GridMap gridMap) {
			gridMap.CreateBlock(4, playerBlock.PositionInt, true);
			AudioManager.Instance.PlayDuplicatorSound(PositionInt);
			return true;
		}
	}
}
