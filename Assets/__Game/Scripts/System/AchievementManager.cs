using System.IO;
using System.Linq;
using UnityEngine;
#if !DISABLESTEAMWORKS
using Steamworks;

#endif

namespace FG {
    public class AchievementManager : MonoBehaviour {
        public static AchievementManager Instance { get; private set; }

        private string[] _achievementLevelGroups = {"boulders", "hills", "pillars", "flatlands"};

        private void OnGameStateChange(GameManager.GameState gameState) {
            if (gameState != GameManager.GameState.GameWon) {
                return;
            }

            UpdateLevelsCompletedAchievements();
            UpdateLevelGroupsCompletedAchievements();

#if !DISABLESTEAMWORKS
            SteamUserStats.StoreStats();
#endif
        }

        private void UpdateLevelsCompletedAchievements() {
#if !DISABLESTEAMWORKS
            if (!File.Exists(GameManager.Instance.CurrentLevelSavePath) ||
                !SteamUserStats.GetStat("levels_completed", out int statValue)) return;

            statValue++;
            SteamUserStats.SetStat("levels_completed", statValue);
#endif
        }

        private void UpdateLevelGroupsCompletedAchievements() {
#if !DISABLESTEAMWORKS
            if ((!File.Exists(GameManager.Instance.CurrentLevelSavePath) &&
                 LevelUtility.GetDiffLevelSavesToLevelCount($"{GameSettings.Instance.LevelFolder}{GameManager.Instance.CurrentLevelGroup}") != 0)) return;

            if (_achievementLevelGroups.Contains(GameManager.Instance.CurrentLevelGroup)) {
                SteamUserStats.SetAchievement($"{GameManager.Instance.CurrentLevelGroup.ToLower()}_complete");
            }
#endif
        }

        private void OnDestroy() {
            if (Instance) {
                GameManager.Instance.gameStateEvent.RemoveListener(OnGameStateChange);
            }
        }

        private void Awake() {
            if (Instance) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            GameManager.Instance.gameStateEvent.AddListener(OnGameStateChange);
        }
    }
}