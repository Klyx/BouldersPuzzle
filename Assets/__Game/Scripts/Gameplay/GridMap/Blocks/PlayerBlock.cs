using UnityEngine;

namespace FG.Gridmap {
    [RequireComponent(typeof(Renderer))]
    public class PlayerBlock : Block {
        [SerializeField] private Material _activeBlockMaterial;
        [SerializeField] private Material _inactiveBlockMaterial;

        private Renderer _renderer;

        public override string BlockName { get; set; } = "Player";
        public override int Priority { get; protected set; } = 10;

        public override bool DisruptConnection {
            get => false;
            protected set => IsActive = value;
        }

        public bool IsActive {
            get => _renderer.sharedMaterial == _activeBlockMaterial;
            set => _renderer.sharedMaterial = value ? _activeBlockMaterial : _inactiveBlockMaterial;
        }

        public override bool ConnectWithPlayer(PlayerBlock playerBlock, GridMap gridMap) {
            if (IsActive) {
                return false;
            }

            IsActive = true;
            return false;
        }

        public override void DisconnectFromPlayer(int order, GridMap gridMap) {
            IsActive = false;
        }

        private new void Awake() {
            base.Awake();
            _renderer = GetComponent<Renderer>();
        }
    }
}