using UnityEngine;

namespace FG {
    public static class Vector3Extensions {
        public static Vector3Int ToVector3Int(this Vector3 source) {
            return new Vector3Int(Mathf.RoundToInt(source.x),
                Mathf.RoundToInt(source.y), Mathf.RoundToInt(source.z));
        }

        public static Vector3Int ToGridPosition(this Vector3 source) {
            return new Vector3Int(Mathf.FloorToInt(source.x), Mathf.FloorToInt(source.y),
                Mathf.FloorToInt(source.z));
        }
        
        public static Vector3 Flattened(this Vector3 source) {
            return new Vector3(source.x, 0f, source.z);
        }
        
        public static Vector3 DirectionTo(this Vector3 source, Vector3 destination) {
            return Vector3.Normalize(destination - source);
        }
    }
}