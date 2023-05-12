using System.Globalization;
using UnityEngine;
using UnityEditor;

namespace FG {
    public class MovementGizmo {
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Active)]
        private static void DrawMovementGizmos(Movement movement, GizmoType gizmoTyp) {
            if (EditorApplication.isPlaying && movement._playerBlocks.Count > 0) {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.black;

               DrawPlayerBlocks(movement, style);
            }
        }

        private static void DrawPlayerBlocks(Movement movement, GUIStyle style) {
            Color previousColor = Gizmos.color;
            Gizmos.color = Color.magenta;
            for (int i = 0; i < movement._playerBlocks.Count; i++) {
                Transform blockTransform = movement._playerBlocks[i].transform;
                Gizmos.DrawWireCube(blockTransform.position, Vector3.one);
                Handles.Label(blockTransform.position, i.ToString(CultureInfo.InvariantCulture), style);
            }

            Gizmos.color = previousColor;
        }
    }
}