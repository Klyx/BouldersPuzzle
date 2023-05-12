using System.Globalization;
using TMPro;
using UnityEngine;

namespace FG {
    public class PlayDone : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _bestStepsText;
        [SerializeField] private TextMeshProUGUI _bestMovesText;
        [SerializeField] private TextMeshProUGUI _bestTimeText;

        private int _numberOfPlays = 0;

        public void SaveOnPlayDone() {
            if (GameManager.Instance.CurrentGameState != GameManager.GameState.GameWon) {
                return;
            }
            
            float currentScore =
                GameManager.Instance.Steps + GameManager.Instance.Moves + GameManager.Instance.PlayTime;
            float bestScore = 0f;
            
            if (GameSave.Instance.GetLevelInfo(GameManager.Instance.CurrentLevelSavePath,
                out GameSave.LevelPlayInfo levelPlayInfo)) {
                _numberOfPlays = levelPlayInfo.numberOfPlays;
                _bestStepsText.text = levelPlayInfo.numberOfSteps.ToString(CultureInfo.InvariantCulture);
                _bestMovesText.text = levelPlayInfo.numberOfMoves.ToString(CultureInfo.InvariantCulture);

                StringUtility.TimeToString(levelPlayInfo.playTime, out string time);
                _bestTimeText.text = time;

                bestScore = levelPlayInfo.numberOfSteps + levelPlayInfo.numberOfMoves + levelPlayInfo.playTime;
            }

            levelPlayInfo.numberOfPlays++;
            if (Mathf.Approximately(bestScore, 0f) || currentScore < bestScore) {
                levelPlayInfo.numberOfSteps = GameManager.Instance.Steps;
                levelPlayInfo.numberOfMoves = GameManager.Instance.Moves;
                levelPlayInfo.playTime = GameManager.Instance.PlayTime;
            }
            
            GameSave.LevelPlayInfo.Save(GameManager.Instance.CurrentLevelSavePath, in levelPlayInfo);
        }
    }
}