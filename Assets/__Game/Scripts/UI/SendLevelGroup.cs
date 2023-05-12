using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FG {
    [RequireComponent(typeof(CanvasGroup))]
    public class SendLevelGroup : MonoBehaviour {
        [SerializeField] private TMP_InputField _levelGroupText;
        [SerializeField] private TextMeshProUGUI _levelGroupValue;
        [SerializeField] private TextMeshProUGUI _levelCountValue;
        [SerializeField] private TextMeshProUGUI _levelGroupCreator;
        [SerializeField] private TextMeshProUGUI _levelLevelGroupVersion;
        [SerializeField] private TMP_InputField _email;
        [SerializeField] private TMP_InputField _message;
        [SerializeField] private Button _sendButton;
        [SerializeField] private Button _backButton;

        private CanvasGroup _canvasGroup;
        private bool _isFileUploaded;

        public bool IsSending { get; private set; }

        public void OnShow() {
            LoadScene.Instance.StartFade(1f, _canvasGroup, OnFadeDone);
            string levelGroupPath = ($"{GameSettings.Instance.LevelFolder}{_levelGroupText.text}");
            _levelCountValue.text =
                LevelUtility.CountLevelsInGroup(levelGroupPath).ToString(CultureInfo.InvariantCulture);

            string infoPath = $"{levelGroupPath}{Path.AltDirectorySeparatorChar}info.txt";
            if (File.Exists(infoPath)) {
                string[] info = File.ReadAllText(infoPath)
                    .Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);

                _levelGroupValue.text = info[0];
                _levelGroupCreator.text = info[1];
                _levelLevelGroupVersion.text =
                    float.Parse(info[2], NumberStyles.Float, CultureInfo.InvariantCulture)
                        .ToString(CultureInfo.InvariantCulture);
            }
        }

        public void OnSend() {
            if (IsSending) {
                return;
            }

            StartCoroutine(PerformSend());
        }

        public void OnBack() {
            LoadScene.Instance.StartFade(0f, _canvasGroup);
        }

        public void OnEmailEditDone(string email) {
            if (string.IsNullOrEmpty(email)) {
                _sendButton.interactable = false;
            }
            else {
                _sendButton.interactable = true;
            }
        }

        private IEnumerator PerformSend() {
            string sourceFolder =
                ($"{GameSettings.Instance.LevelFolder}{_levelGroupValue.text}{Path.AltDirectorySeparatorChar}")
                .ToLower();
            string fileOutput =
                $"{Application.temporaryCachePath}{Path.AltDirectorySeparatorChar}{_levelGroupValue.text}.pfg".ToLower();

            _sendButton.interactable = false;
            _backButton.interactable = false;

            yield return StartCoroutine(PackFiles(sourceFolder, fileOutput));

            _isFileUploaded = false;
            UploadFile(fileOutput);
            yield return new WaitUntil(() => _isFileUploaded);

            if (File.Exists(fileOutput)) {
                File.Delete(fileOutput);
            }
            
            _backButton.interactable = true;
        }

        private IEnumerator PackFiles(string sourceFolder, string fileOutput) {
            using (FileStream stream = File.OpenWrite(fileOutput)) {
                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode)) {
                    List<string> filesToPack =
                        new List<string>(Directory.GetFiles(sourceFolder, "*.pmap", SearchOption.TopDirectoryOnly));

                    if (File.Exists($"{sourceFolder}info.txt")) {
                        filesToPack.AddRange(
                            Directory.GetFiles(sourceFolder, "info.txt", SearchOption.TopDirectoryOnly));
                    }
                    
                    string msgFileOutput =
                        $"{Application.temporaryCachePath}{Path.AltDirectorySeparatorChar}{_levelGroupValue.text}msg.txt".ToLower();
                    File.WriteAllText(msgFileOutput, $"{_email.text}\n{_message.text}");
                    filesToPack.Add(msgFileOutput);
                    
                    writer.Write(filesToPack.Count);
                    foreach (string file in filesToPack) {
                        byte[] bytes = File.ReadAllBytes(file);
                        writer.Write(Path.GetFileName(file));
                        writer.Write(bytes.Length);
                        writer.Write(bytes);
                        yield return null;
                    }

                    if (File.Exists(msgFileOutput)) {
                        File.Delete(msgFileOutput);
                    }
                }
            }
        }
        
        private void UploadFile(string file) {
            string host =
                Encoding.UTF8.GetString(
                    Convert.FromBase64String("ZnRwOi8vZnRwLmZhcmV3ZWxsZ2FtZXMuc2UvQm91bGRlcnNQdXp6bGUv"));
            string user =
                Encoding.UTF8.GetString(Convert.FromBase64String("Q29tbXVuaXR5VXBsb2Fkc0BmYXJld2VsbGdhbWVzLnNl"));
            string pass = Encoding.UTF8.GetString(
                Convert.FromBase64String("cVdZWWZFZzhrQmJWcFduNENXeXBXYUdJRDdsNW9IdkNxSkZ4WU5DMDNIZExGWGVmZ1dx"));

            using (WebClient client = new WebClient()) {
                client.Credentials = new NetworkCredential(user, pass);
                client.UploadFileCompleted += new UploadFileCompletedEventHandler(OnFileUploadCompleted);
                client.UploadFileTaskAsync($"{host}{_levelGroupText.text}_{DateTime.Now.Ticks}.pfg",
                    WebRequestMethods.Ftp.UploadFile, file);
            }
        }
        
        private void OnFileUploadCompleted(object sender, UploadFileCompletedEventArgs e) {
            _isFileUploaded = true;
            // Todo - Report error
            // https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.asynccompletedeventargs.error?view=net-5.0#System_ComponentModel_AsyncCompletedEventArgs_Error
        }

        private void OnFadeDone() {
            if (_canvasGroup.interactable) {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
                return;
            }

            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        private void Awake() {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
    }
}