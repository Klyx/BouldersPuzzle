using System.Collections.Generic;
using FG.Gridmap;
using UnityEngine;

namespace FG {
    public class DistanceComparer : IComparer<Block> {
        private Vector3 _targetPosition;

        public DistanceComparer(Vector3 targetPositionPosition) {
            _targetPosition = targetPositionPosition;
        }

        public int Compare(Block a, Block b) {
            return Vector3.Distance(a.transform.position, _targetPosition).CompareTo(Vector3.Distance(b.transform.position, _targetPosition));
        }
    }
}