namespace CliFrontend.Util;

using CliFrontend.Data;

public static class Extensions
{
	public static DateTime RoundToQuarterHour(this DateTime dateTime)
	{
		int minutes = dateTime.Minute;
		int roundedMinutes = (int)(Math.Round(minutes / 15.0) * 15);
		return new DateTime(
				dateTime.Year,
				dateTime.Month,
				dateTime.Day,
				dateTime.Hour,
				roundedMinutes,
				0,
				dateTime.Kind);
	}


	public static bool HasEndTime(this Entry entry)
		=> entry.End != null;
}
