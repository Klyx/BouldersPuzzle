using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FG {
	public class EditorUploadLevelGroup : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI _levelGroupText;
		[SerializeField] private TMP_InputField _email;
		[SerializeField] private TMP_InputField _name;
		[SerializeField] private TMP_InputField _message;
		[SerializeField] private Button _sendButton;
		[SerializeField] private Button _backButton;
		
		private bool _isFileUploaded;
		
		public bool IsSending { get; private set; }

		public void OnSend() {
			if (IsSending) {
				return;
			}

			StartCoroutine(PerformSend());
		}

		private IEnumerator PerformSend() {
			_sendButton.interactable = false;
			_backButton.interactable = false;
			
			string sourceFolder =
				($"{GameSettings.Instance.LevelFolder}{_levelGroupText.text}{Path.AltDirectorySeparatorChar}")
				.ToLower();
			string fileOutput =
				$"{Application.temporaryCachePath}{Path.AltDirectorySeparatorChar}{_levelGroupText.text}.pfg".ToLower();
			
			yield return StartCoroutine(PackFiles(sourceFolder, fileOutput));
			
			_isFileUploaded = false;
			UploadFile(fileOutput);
			yield return new WaitUntil(() => _isFileUploaded);
			
			if (File.Exists(fileOutput)) {
				File.Delete(fileOutput);
			}
            
			_backButton.interactable = true;
		}
		
		private void UploadFile(string file) {
			string host =
				Encoding.UTF8.GetString(
					Convert.FromBase64String("SecretString"));
			string user =
				Encoding.UTF8.GetString(Convert.FromBase64String("SecretString"));
			string pass = Encoding.UTF8.GetString(
				Convert.FromBase64String("SecretString"));

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
						$"{Application.temporaryCachePath}{Path.AltDirectorySeparatorChar}{_levelGroupText .text}msg.txt".ToLower();
					File.WriteAllText(msgFileOutput, $"{_email.text}\n{_name.text}\n{_message.text}");
					filesToPack.Add(msgFileOutput);
                    
					writer.Write(filesToPack.Count);
					foreach (string file in filesToPack) {
						byte[] bytes = File.ReadAllBytes(file);
						writer.Write(Path.GetFileName(file));
						writer.Write(bytes.Length);
						writer.Write(bytes);
						yield return null;
					}
					writer.Close();

					if (File.Exists(msgFileOutput)) {
						File.Delete(msgFileOutput);
					}
				}
				stream.Close();
			}
		}
	}
}
