namespace FG.Gridmap {
    public class GoalBlock : Block {
        public bool IsConnectedToPlayer { get; private set; }

        public static int GoalCount { get; private set; }
        public static int ConnectedGoals { get; private set; }

        public override string BlockName { get; set; } = "Goal";
        public override int Priority { get; protected set; } = 20;
        public override bool DisruptConnection { get; protected set; } = false;

        public override bool ConnectWithPlayer(PlayerBlock playerBlock, GridMap gridMap) {
            if (IsConnectedToPlayer) {
                return false;
            }
            
            ConnectedGoals++;
            IsConnectedToPlayer = true;

            if (ConnectedGoals >= GoalCount) {
                GameManager.Instance.CurrentGameState = GameManager.GameState.GameWon;
            }

            return false;
        }

        public override void DisconnectFromPlayer(int order, GridMap gridMap) {
            if (!IsConnectedToPlayer) {
                return;
            }
            
            ConnectedGoals--;
            IsConnectedToPlayer = false;
        }

        private void OnEnable() {
            GoalCount++;
        }

        private void OnDestroy() {
            if (IsConnectedToPlayer) {
                ConnectedGoals--;
            }
            GoalCount--;
        }
    }
}