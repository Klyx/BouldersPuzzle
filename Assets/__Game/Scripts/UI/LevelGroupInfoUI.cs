using System.Globalization;
using TMPro;
using UnityEngine;

namespace FG {
    public class LevelGroupInfoUI : MonoBehaviour {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _creatorValueLabel;
        [SerializeField] private TextMeshProUGUI _versionValueLabel;
        [SerializeField] private TextMeshProUGUI _numberOfLevelsValueLabel;
        
        private string _name;
        private string _creator;
        private float _version;
        private int _numberOfLevels;

        public string FolderName { get; set; }
        public string Name { get; set; }

        public string Creator {
            get => _creator;
            set
            {
                _creator = value;
                _creatorValueLabel.text = _creator;
            }
        }

        public float Version {
            get => _numberOfLevels;
            set
            {
                _version = value;
                _versionValueLabel.text = _version.ToString("F2", CultureInfo.InvariantCulture);
            }
        }

        public int NumberOfLevels {
            get => _numberOfLevels;
            set
            {
                _numberOfLevels = value;
                _numberOfLevelsValueLabel.text = _numberOfLevels.ToString();
            }
        }
    }
}