using UnityEngine;

namespace FG {
	public static class Vector2Extensions {
		public static Vector2 DirectionTo(this Vector2 source, Vector2 destination) {
			return (destination - source);
		}
	}
}
