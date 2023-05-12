using System.IO;
using UnityEditor;
using UnityEngine;

namespace FG {
	public class GeneralMenuItems {
		[MenuItem("Farewell/Open level group folder...")]
		private static void OpenLevelGroupFolder()
		{
			Application.OpenURL($"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}Levels{Path.AltDirectorySeparatorChar}");
		}
	}
}
