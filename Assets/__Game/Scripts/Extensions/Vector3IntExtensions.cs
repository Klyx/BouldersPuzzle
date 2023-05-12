using UnityEngine;

namespace FG {
    public static class Vector3IntExtensions {
        public static Vector3 ToVector3(this Vector3Int source) {
            return new Vector3Int(source.x, source.y, source.z);
        }
    }
}