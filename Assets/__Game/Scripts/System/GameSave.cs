using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FG {
	[Serializable]
	public class GameSave {
		[Serializable]
		public struct LevelPlayInfo {
			public int numberOfPlays;
			public int numberOfSteps;
			public int numberOfMoves;
			public float playTime;
			
			public float TotalScore => numberOfSteps + numberOfMoves + playTime;

			public static void Save(in string file, in LevelPlayInfo levelPlayInfo) {
				BinaryFormatter formatter = new BinaryFormatter();
				Stream stream =
					new FileStream(file, FileMode.Create, FileAccess.Write);
				formatter.Serialize(stream, levelPlayInfo);
				stream.Close();
			}

			public static bool Load(in string file, out LevelPlayInfo levelInfo) { ;
				if (File.Exists(file)) {
					BinaryFormatter formatter = new BinaryFormatter();
					Stream stream =
						new FileStream(file, FileMode.Open, FileAccess.Read);
					levelInfo = (LevelPlayInfo) formatter.Deserialize(stream);
					stream.Close();

					return true;
				}

				levelInfo = new LevelPlayInfo();
				return false;
			}
		}
		
		public static GameSave Instance {
			get
			{
				if (ReferenceEquals(_instance, null)) {
					LoadSave();
				}

				return _instance;
			}
			private set { _instance = value; }
		}
		
		[field: NonSerialized] private static GameSave _instance;
		
		public int LastSelectedLevelGroupIndex { get; set; } = 0;
		
		private Dictionary<string, int> _lastSelectedLevelIndex = new Dictionary<string, int>();

		public void SetLastSelectedLevelOfGroup(in string levelGroup, int index) {
			_lastSelectedLevelIndex[levelGroup] = index;
		}

		public bool GetLastSelectedLevelOfGroup(in string levelGroup, out int index) {
			if (_lastSelectedLevelIndex.ContainsKey(levelGroup)) {
				index = _lastSelectedLevelIndex[levelGroup];
				return true;
			}

			index = -1;
			return false;
		}

		public bool GetLevelInfo(in string level, out LevelPlayInfo levelInfo) {
			if (LevelPlayInfo.Load(level, out levelInfo)) {
				return true;
			}

			levelInfo = new LevelPlayInfo();
			return false;
		}
		
		public static void LoadSave() {
			if (File.Exists($"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}settings.sav")) {
				BinaryFormatter formatter = new BinaryFormatter();
				Stream stream =
					new FileStream($"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}settings.sav",
						FileMode.Open, FileAccess.Read);
				_instance = (GameSave) formatter.Deserialize(stream);
				stream.Close();
			}
			else {
				_instance = new GameSave();
			}
		}

		public static void SaveSettings() {
			if (ReferenceEquals(_instance, null)) {
				_instance = new GameSave();
			}
            
			BinaryFormatter formatter = new BinaryFormatter();
			Stream stream =
				new FileStream($"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}settings.sav",
					FileMode.Create, FileAccess.Write);
			formatter.Serialize(stream, _instance);
			stream.Close();
		}
	}
}
