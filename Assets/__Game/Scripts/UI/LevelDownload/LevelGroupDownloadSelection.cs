using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
#if !UNITY_WEBGL
using UnityEngine.Networking;
#endif

namespace FG {
    public class LevelGroupDownloadSelection : MonoBehaviour {
        [SerializeField] private LevelGroupDownloadInfoUI _levelGroupInfoPrefab;
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private CanvasGroup _fadeCanvasGroup;
        [SerializeField] private SelectionSlider _selectionSlider;

        public void OnLevelGroupChanged(int index, GameObject option) {
            _selectionSlider.SetOptionLabelText(_contentTransform.GetChild(index)
                ?.GetComponent<LevelGroupDownloadInfoUI>()?.LevelGroupName);
        }

#if (!FG_DEMO && !UNITY_WEBGL)

        private IEnumerator AddLevelGroupInfo() {
            using (UnityWebRequest webRequest =
                UnityWebRequest.Get(
                    $"https://boulders.farewellgames.se/get_levelGroupsInfo.php{GameSettings._netAccessCode}")
            ) {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success) {
                    string[] lines =
                        webRequest.downloadHandler.text.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines) {
                        LevelGroupDownloadInfoUI levelGroupInfo = Instantiate(_levelGroupInfoPrefab, _contentTransform)
                            ?.GetComponent<LevelGroupDownloadInfoUI>();
                        if (levelGroupInfo) {
                            string[] info = line.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                            //Name:Creator:Version:Folder:Count
                            levelGroupInfo.LevelGroupName = info[0];
                            levelGroupInfo.Creator = info[1];
                            levelGroupInfo.Version = float.Parse(info[2], CultureInfo.InvariantCulture);
                            levelGroupInfo.Folder = info[3];
                            levelGroupInfo.LevelCount = int.Parse(info[4], CultureInfo.InvariantCulture);
                        }
                    }

                    _selectionSlider.SetOptionLabelText(_contentTransform.GetChild(0)
                        ?.GetComponent<LevelGroupDownloadInfoUI>()?.LevelGroupName);
                    _selectionSlider.SetCurrentIndex(0);
                }
                else if (webRequest.result != UnityWebRequest.Result.InProgress) {
                    // Todo handle error
                    Debug.LogError(webRequest.error);
                }
            }

            LoadScene.Instance.StartFade(0f, _fadeCanvasGroup);

            yield return null;
        }

        private void Awake() {
            StartCoroutine(AddLevelGroupInfo());
        }

#endif // !FG_DEMO
    }
}