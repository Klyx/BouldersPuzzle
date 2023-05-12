using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

#if !UNITY_WEBGL
using UnityEngine.Networking;
#endif

namespace FG {
    public class LevelDownloadActionPanel : MonoBehaviour {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _downloadButton;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private CanvasGroup _fadeCanvas;

        private string _levelGroup;
        private string _levelGroupNetFolder;

        public void OnLevelGroupChanged(int index, GameObject option) {
            LevelGroupDownloadInfoUI levelGroupInfo = option.GetComponent<LevelGroupDownloadInfoUI>();
            Assert.IsNotNull(levelGroupInfo, "levelGroupInfo != null");

            float installedVersion = LevelGroupUtility.LevelGroupVersionInstalled(levelGroupInfo.Folder);
            if (installedVersion < levelGroupInfo.Version) {
                _downloadButton.interactable = true;
                if (installedVersion < 0) {
                    Localization.Instance.GetText("Download", out string text);
                    _downloadButton.GetComponentInChildren<TextMeshProUGUI>().text = text;

                    _playButton.interactable = false;
                    _deleteButton.interactable = false;
                }
                else {
                    Localization.Instance.GetText("Update", out string text);
                    _downloadButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
                    _playButton.interactable = true;
                    _deleteButton.interactable = true;
                }
            }
            else {
                _downloadButton.interactable = false;
                _playButton.interactable = true;
                _deleteButton.interactable = true;
            }

            _levelGroup = levelGroupInfo.LevelGroupName;
            _levelGroupNetFolder = levelGroupInfo.Folder;
        }

        public void OnPlayClick() {
            GameManager.Instance.CurrentLevelGroup = _levelGroup;
            LevelUtility.GetLevelName(_levelGroup, 0, out string levelName);
            GameManager.Instance.CurrentLevel = levelName;
            GameManager.Instance.CurrentLevelIndex = 0;
            GameSave.Instance.LastSelectedLevelGroupIndex = LevelGroupUtility.GetLevelGroupIndex(_levelGroup);
            GameSave.Instance.SetLastSelectedLevelOfGroup(_levelGroup, 0);

            LoadScene.Instance.StartLoadScene(1, _fadeCanvas);
        }

        public void OnDownloadClick() {
            LevelUtility.GetLevelFilenames($"{GameSettings.Instance.LevelFolder}{_levelGroup}",
                out List<string> levelFiles);
            foreach (string levelFile in levelFiles) {
                File.Delete(levelFile);
            }

#if !UNITY_WEBGL
            StartCoroutine(DownloadLevelGroup());
#endif
        }

        public void OnDeleteClick() {
            LevelGroupUtility.DeleteLevelGroup(_levelGroup);
            _downloadButton.interactable = true;
            _playButton.interactable = false;
            _deleteButton.interactable = false;

            Localization.Instance.GetText("Download", out string text);
            _downloadButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }

#if !UNITY_WEBGL
        private IEnumerator DownloadLevelGroup() {
            _playButton.interactable = false;
            _downloadButton.interactable = false;
            
            WWWForm form = new WWWForm();
            form.AddField("levelGroupFolder", _levelGroupNetFolder);
            using (UnityWebRequest webRequest = UnityWebRequest.Post(
                $"https://boulders.farewellgames.se/get_levelGroupFiles.php{GameSettings._netAccessCode}",
                form)) {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success) {
                    string[] files =
                        webRequest.downloadHandler.text.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);

                    string filePath =
                        $"{GameSettings.Instance.LevelFolder}{_levelGroupNetFolder}{Path.AltDirectorySeparatorChar}";
                    string netFilePath = $"https://boulders.farewellgames.se/levels/{_levelGroupNetFolder}/";
                    foreach (string file in files) {
                        using (UnityWebRequest downloadWebRequest = new UnityWebRequest(
                            $"{netFilePath}{file}{GameSettings._netAccessCode}")) {
                            downloadWebRequest.downloadHandler = new DownloadHandlerFile(Path.Combine(filePath, file));
                            yield return downloadWebRequest.SendWebRequest();
                        }
                    }
                }
                else if (webRequest.result != UnityWebRequest.Result.InProgress) {
                    // Todo handle error
                    Debug.LogError(webRequest.error);
                }
            }

            _playButton.interactable = true;
            _deleteButton.interactable = true;
        }
#endif
    }
}