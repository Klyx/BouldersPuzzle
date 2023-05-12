namespace FG {
	public static class StringUtility {
		public static void TimeToString(float time, out string timeString) {
			int minutes = (int) (time / 60f);
			int seconds = (int) (time % 60f);
			timeString = $"{minutes:00}:{seconds:00}";
		}
	}
}
