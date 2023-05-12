using UnityEngine;

namespace FG.Gridmap {
    public abstract class Block : MonoBehaviour {
        protected Transform _transform;

        public int PrefabIndex { get; set; }
        public abstract int Priority { get; protected set; }

        public Vector3Int PositionInt {
            get => _transform.position.ToVector3Int();
            set => _transform.position = value;
        }

        public abstract bool DisruptConnection { get; protected set; }

        public abstract string BlockName { get; set; }

        /// <summary>
        /// Returns true if player block should be deleted.
        /// </summary>
        public abstract bool ConnectWithPlayer(PlayerBlock playerBlock, GridMap gridMap);
        public virtual void DisconnectFromPlayer(int order, GridMap gridMap) {}

        protected virtual void Awake() {
            _transform = transform;
        }
    }
}