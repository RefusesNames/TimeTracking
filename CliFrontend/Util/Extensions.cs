namespace CliFrontend.Util;

using CliFrontend.Data;

public static class Extensions
{
	public static DateTime RoundToQuarterHour(this DateTime dateTime)
	{
		int minutes = dateTime.Minute;
		int roundedMinutes = (int)(Math.Round(minutes / 15.0) * 15);
		int difference = roundedMinutes - minutes;
		
		return dateTime
			+ TimeSpan.FromMinutes(difference)
			- TimeSpan.FromSeconds(dateTime.Second);
	}


	public static bool HasEndTime(this Entry entry)
		=> entry.End != null;
}
