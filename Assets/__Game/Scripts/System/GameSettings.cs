using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FG {
    [Serializable]
    public class GameSettings {
        public static GameSettings Instance {
            get
            {
                if (ReferenceEquals(_instance, null)) {
                    LoadSettings();
                }

                return _instance;
            }
            private set { _instance = value; }
        }

        [field: NonSerialized] private static GameSettings _instance;

        [field: NonSerialized] public string LevelFolder { get; private set; } = string.Empty;

        [NonSerialized] public const string _netAccessCode = "?secretkey=SecretString";
        
        public static int SettingsVersion { get; set; } = 3;

        public bool IsFirstStart { get; set; } = true;

        public float WaitBeforeLoose { get; set; } = 1f;

        // Camera
        public bool ShakeCamera { get; set; } = true;
        public float ShakeCameraMoveDuration { get; set; } = 0.1f;
        public float ShakeCameraMoveAmount { get; set; } = 0.1f;
        public float ShakeCameraCantMoveDuration { get; set; } = 0.2f;
        public float ShakeCameraCantMoveAmount { get; set; } = 0.07f;
        public float CameraKeyboardZoomSpeed { get; set; } = 10f;

        // UI
        public float ScrollRectTimeToReachItem { get; set; } = 0.5f;
        public float SceneFadeTime { get; set; } = 0.2f;
        public float MenuFadeTime { get; set; } = 0.5f;

        public float CameraSnapToPositionTime { get; set; } = 0.1f;
        public float CameraSnapToRotationTime { get; set; } = 0.1f;

        public float BlockRotationTime { get; set; } = 0.3f;

        public bool ShowStepsUI { get; set; } = true;
        public bool ShowMovesUI { get; set; } = true;
        public bool ShowTimeUI { get; set; } = true;
        public bool ShowGoalsUI { get; set; } = true;

        public int Language { get; set; } = 0;

        // Input
        public float CameraInvertLook { get; set; } = -1f;
        public float MinimumSwipeDistance { get; set; } = 0.2f;
        public float MaximumSwipeTime { get; set; } = 1.0f;
        public float SwipeDirectionThreshold { get; set; } = 0.9f;

        // Audio
        public float MasterVolume { get; set; } = 1f;
        public float MusicVolume { get; set; } = 1f;
        public float SFXVolume { get; set; } = 1f;
        public float UIVolume { get; set; } = 1f;

        // Graphics
        public bool Fullscreen { get; set; } = true;
#if UNITY_ANDROID
        public bool MergeMeshes { get; set; } = true;
#else
        public bool MergeMeshes { get; set; }
#endif
        public int VSync { get; set; }

#if !UNITY_ANDROID
        public int Resolution { get; set; } = -1;
#endif

        // Mobile
#if (UNITY_IOS || UNITY_ANDROID)
        public bool BoughtFullVersion { get; set; } = false;
        
        public bool FirstPlayThisSession { get; set; } = true;

        [NonSerialized] public const bool AdsTestMode = false;
        [NonSerialized] public const string PlayStoreID = "4300689";
        [NonSerialized] public const string PlayStorePlacement = "Rewarded_Android";
#endif
        
        // Level editor
        public float EditorCameraMoveSpeed { get; set; } = 10f;
        public float EditorCameraRotateSpeed { get; set; } = 100f;
        public float EditorMinSelectionDistance { get; set; } = 2f;
        public float EditorMaxSelectionDistance { get; set; } = 20f;
        public int EditorMaxUndos { get; set; } = 20;

        // Accessibility
        public bool ShowBlockSymbols { get; set; } = false;

        public static void LoadSettings() {
            ResetSettingsIfNeeded();
            
            if (File.Exists($"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}settings.cfg")) {
                BinaryFormatter formatter = new BinaryFormatter();
                Stream stream =
                    new FileStream($"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}settings.cfg",
                        FileMode.Open, FileAccess.Read);
                _instance = (GameSettings)formatter.Deserialize(stream);
                stream.Close();
            }
            else {
                _instance = new GameSettings();
                _instance.GetUnsavedSettings();
            }

            _instance.LevelFolder =
                $"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}Levels{Path.AltDirectorySeparatorChar}";

            _instance.ApplySettings();
        }

        public static void SaveSettings() {
            if (ReferenceEquals(_instance, null)) {
                _instance = new GameSettings();
            }

            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream =
                new FileStream($"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}settings.cfg",
                    FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, _instance);
            stream.Close();
        }

        public void ApplySettings() {
#if !UNITY_ANDROID
            UnityEngine.Resolution resolution = Screen.currentResolution;
            if (Resolution >= 0 && Resolution < Screen.resolutions.Length) {
                resolution = Screen.resolutions[Resolution];
            }
            Screen.SetResolution(resolution.width, resolution.height, Fullscreen, resolution.refreshRate);
#endif

            QualitySettings.vSyncCount = VSync;

            AudioManager.Instance.SetVolume();
        }

        private static void ResetSettingsIfNeeded() {
            string file = $"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}settings.ver";
            if (File.Exists(file)) {
                StreamReader reader = new StreamReader(file);
                string versionLine = reader.ReadLine();
                reader.Close();
                if (int.TryParse(versionLine, out int version)) {
                    if (version < SettingsVersion) {
                        SaveSettings();
                        WriteSettingsVersion();
                    }
                }
            }
            else {
                SaveSettings();
                WriteSettingsVersion();
            }

            void WriteSettingsVersion() {
                StreamWriter writer = new StreamWriter(file);
                writer.WriteLine(SettingsVersion);
                writer.Close();
            }
        }

        private void GetUnsavedSettings() {
            Fullscreen = Screen.fullScreen;
            VSync = QualitySettings.vSyncCount;

#if !UNITY_ANDROID
            Resolution = Array.IndexOf(Screen.resolutions, Screen.currentResolution);
#endif
        }
    }
}
