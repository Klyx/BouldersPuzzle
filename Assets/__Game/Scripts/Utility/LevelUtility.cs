using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

namespace FG {
    public static class LevelUtility {
        public static int GetDiffLevelSavesToLevelCount(in string levelGroupPath) {
            if (!Directory.Exists(levelGroupPath)) {
                return -1;
            }

            int levelCount = Directory.GetFiles(levelGroupPath, "*.pmap", SearchOption.TopDirectoryOnly).Length;
            int saveCount = Directory.GetFiles(levelGroupPath, "*.sav", SearchOption.TopDirectoryOnly).Length;
            
            return levelCount - saveCount;
        }
        
        public static void GetLevelFilenames(in string levelGroupPath, out List<string> levels) {
            levels = new List<string>();

            if (!Directory.Exists(levelGroupPath)) {
                return;
            }
            
            levels.AddRange(Directory.GetFiles(levelGroupPath, "*.pmap", SearchOption.TopDirectoryOnly));
            levels.Sort();
        }

        public static void GetLevelNames(in string levelGroupPath, ref List<string> levels) {
            if (!Directory.Exists(levelGroupPath)) {
                return;
            }
            
            levels.AddRange(Directory.GetFiles(levelGroupPath, "*.pmap", SearchOption.TopDirectoryOnly)
                .Select(file => Path.GetFileNameWithoutExtension(file)).ToArray());
            levels.Sort();
        }
        
        public static void GetLevelName(in string levelGroup, int index, out string levelName) {
            List<string> levels = new List<string>();
            GetLevelNames($"{GameSettings.Instance.LevelFolder}{levelGroup}", ref levels);
            levelName = levels[index];
        }

        public static int GetLevelIndex(in string levelGroup, in string level) {
            List<string> levels = new List<string>();
            GetLevelNames($"{GameSettings.Instance.LevelFolder}{levelGroup}", ref levels);
            return levels.IndexOf(level);
        }
        
        public static int CountLevelsInGroup(in string levelGroupPath) {
            if (!Directory.Exists(levelGroupPath)) {
                return 0;
            }

            return Directory.GetFiles(levelGroupPath, "*.pmap", SearchOption.TopDirectoryOnly).Length;
        }
    }
}