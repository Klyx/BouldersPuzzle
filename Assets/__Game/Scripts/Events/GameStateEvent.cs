using System;
using UnityEngine.Events;

namespace FG {
	[Serializable]
	public class GameStateEvent : UnityEvent<GameManager.GameState> {
	}
}
