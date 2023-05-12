using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FG {
    public static class LevelGroupUtility {
        public static int GetLevelGroupIndex(in string levelGroup) {
            List<string> levels = new List<string>();
            List<string> levelGroups = new List<string>();
            GetLevelGroupNames(ref levelGroups);
            return levelGroups.IndexOf(levelGroup);
        }

        /// <summary>
        /// Returns negative value if not installed.
        /// </summary>
        public static float LevelGroupVersionInstalled(in string folder) {
            string levelGroupInfoFile =
                $"{GameSettings.Instance.LevelFolder}{folder}{Path.AltDirectorySeparatorChar}info.txt";
            if (File.Exists(levelGroupInfoFile)) {
                string[] info = File.ReadAllText(levelGroupInfoFile)
                    .Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                return float.Parse(info[2], CultureInfo.InvariantCulture);
            }

            return -1f;
        }

        public static void GetLevelGroupFolders(out List<string> levelGroups) {
            levelGroups = new List<string>();

#if FG_DEMO
			levelGroups.Add("Boulders");
#else
            levelGroups.AddRange(Directory.GetDirectories(GameSettings.Instance.LevelFolder)
                .Select(directory => Path.GetFileName(directory)).ToArray());
#endif
        }

        public static void GetLevelGroupNames(ref List<string> levelGroups) {
            List<string> folders = new List<string>();

#if FG_DEMO
			levelGroups.Add("Boulders");
#else
            folders.AddRange(Directory.GetDirectories(GameSettings.Instance.LevelFolder)
                .Select(directory => directory).ToArray());

            foreach (string folder in folders) {
                string levelGroupInfoFile = $"{folder}{Path.AltDirectorySeparatorChar}info.txt";
                string[] info = File.ReadAllText(levelGroupInfoFile)
                    .Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                levelGroups.Add(info[0]);
            }
#endif
        }

        public static bool GetLevelGroupInfo(in string levelGroup, ref LevelGroupInfoUI groupInfoUI) {
            string levelGroupPath = $"{GameSettings.Instance.LevelFolder}{levelGroup}";
            string levelGroupInfoFile = $"{levelGroupPath}{Path.AltDirectorySeparatorChar}info.txt";

            if (File.Exists(levelGroupInfoFile)) {
                string[] info = File.ReadAllText(levelGroupInfoFile)
                    .Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);

                groupInfoUI.Name = info[0];
                groupInfoUI.Creator = info[1];
                groupInfoUI.Version = float.Parse(info[2], NumberStyles.Float, CultureInfo.InvariantCulture);
            }

            groupInfoUI.NumberOfLevels =
                Directory.GetFiles(levelGroupPath, "*.pmap", SearchOption.TopDirectoryOnly).Length;
            return true;
        }

        public static void DeleteLevelGroup(in string levelGroup) {
            if (!Directory.Exists($"{GameSettings.Instance.LevelFolder}{levelGroup}")) {
                return;
            }

            Directory.Delete($"{GameSettings.Instance.LevelFolder}{levelGroup}", true);
        }
    }
}