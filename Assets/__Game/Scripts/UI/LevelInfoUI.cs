using System.Globalization;
using TMPro;
using UnityEngine;

namespace FG {
	public class LevelInfoUI : MonoBehaviour {
		[Header("References")]
		[SerializeField] private TextMeshProUGUI _timesPlayedValueLabel;
		[SerializeField] private TextMeshProUGUI _latestStepsValueLabel;
		[SerializeField] private TextMeshProUGUI _latestMovesValueLabel;
		[SerializeField] private TextMeshProUGUI _latestTimeValueLabel;
		
		private int _timesPlayed;
		private int _steps;
		private int _moves;
		private float _time;
		
		public bool InfoSet { get; set; }
		public string LevelFolder { get; set; }
		public string Name { get; set; }
		
		public int TimesPlayed {
			get => _timesPlayed;
			set
			{
				_timesPlayed = value;
				_timesPlayedValueLabel.text = _timesPlayed.ToString();
			}
		}
		
		public int Steps {
			get => _steps;
			set {
				_steps = value;
				_latestStepsValueLabel.text = _steps.ToString();
			}
		}
		
		public int Moves {
			get => _moves;
			set {
				_moves = value;
				_latestMovesValueLabel.text = _moves.ToString();
			}
		}
		
		public float Time {
			get => _time;
			set {
				_time = value;
				StringUtility.TimeToString(_time, out string time);
				_latestTimeValueLabel.text = time;
			}
		}
	}
}
