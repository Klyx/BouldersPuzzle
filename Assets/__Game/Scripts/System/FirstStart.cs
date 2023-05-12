using System;
using UnityEngine;
using System.IO;

namespace FG {
    public class FirstStart : MonoBehaviour {
        [Serializable]
        public struct LevelSet {
            public string levelFolder;
            public TextAsset infoFileToCopy;
            public TextAsset[] filesToCopy;
        }

        [SerializeField] private LevelSet[] _levelSets;

        private void CopyFiles() {
            foreach (LevelSet levelSet in _levelSets) {
                if (!Directory.Exists($"{GameSettings.Instance.LevelFolder}{levelSet.levelFolder}")) {
                    string[] info =
                        levelSet.infoFileToCopy.text.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    string folderPath =
                        $"{GameSettings.Instance.LevelFolder}{levelSet.levelFolder}{Path.AltDirectorySeparatorChar}";

                    if (!Directory.Exists(folderPath)) {
                        Directory.CreateDirectory(folderPath);
                    }

#if FG_DEMO
                    int i = 0;
#endif
                    File.WriteAllBytes($"{folderPath}{levelSet.infoFileToCopy.name}.txt",
                        levelSet.infoFileToCopy.bytes);
                    foreach (TextAsset textAsset in levelSet.filesToCopy) {
                        File.WriteAllBytes($"{folderPath}{textAsset.name}.pmap", textAsset.bytes);
#if FG_DEMO
                    i++;
                        if (i >= 9) {
                            break;
                        }
#endif
                    }

#if FG_DEMO
                    break;
#endif
                }
            }
        }

        private void Start() {
            if (GameSettings.Instance.IsFirstStart) {
                CopyFiles();

                GameSettings.Instance.IsFirstStart = false;
                GameSettings.SaveSettings();
            }
        }
    }
}