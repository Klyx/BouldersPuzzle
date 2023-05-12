using System.Collections.Generic;
using FG.Gridmap;
using UnityEngine;
using UnityEditor;

namespace FG {
	public class GridmapGizmoDrawer
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        static void DrawGizmoForMyScript(GridMap scr, GizmoType gizmoType)
        {
            Gizmos.color = Color.magenta;
            foreach (KeyValuePair<Vector3Int, Block> pair in scr.Map) {
                if (pair.Value.BlockName.Equals("Player")) {
                    Gizmos.DrawCube(pair.Value.PositionInt, Vector3.one);
                }
            }
            Gizmos.color = Color.white;
        }
    }
}
