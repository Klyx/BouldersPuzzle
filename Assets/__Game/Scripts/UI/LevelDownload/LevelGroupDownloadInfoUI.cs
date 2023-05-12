using System.Globalization;
using TMPro;
using UnityEngine;

namespace FG {
    public class LevelGroupDownloadInfoUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _creatorValue;
        [SerializeField] private TextMeshProUGUI _versionValue;
        [SerializeField] private TextMeshProUGUI _levelCountValue;

        private float _version = -1f;

        public string LevelGroupName { get; set; }

        public string Creator {
            set { _creatorValue.text = value; }
        }

        public float Version {
            get => _version;
            set {
                _version = value;
                _versionValue.text = _version.ToString("F2", CultureInfo.InvariantCulture);
            }
        }

        public string Folder { get; set; }

        public int LevelCount {
            set { _levelCountValue.text = value.ToString(CultureInfo.InvariantCulture); }
        }
    }
}