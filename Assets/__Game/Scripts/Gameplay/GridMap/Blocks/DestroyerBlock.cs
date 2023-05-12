namespace FG.Gridmap {
	public class DestroyerBlock : Block {
		public override string BlockName { get; set; } = "Destroyer";
		public override int Priority { get; protected set; } = 30;
		public override bool DisruptConnection { get; protected set; } = true;

		public override bool ConnectWithPlayer(PlayerBlock playerBlock, GridMap gridMap) {
			AudioManager.Instance.PlayDestroyerSound(PositionInt);
			return true;
		}
	}
}
