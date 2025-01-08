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

	public static bool IsSameMonthAs(this DateTime dateTime1, DateTime dateTime2)
		=> dateTime1.Year == dateTime2.Year
		&& dateTime1.Month == dateTime2.Month;
}
