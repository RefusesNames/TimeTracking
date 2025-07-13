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

	public static bool IsBetween(this Entry entry, DateTime start, DateTime end)
		=> entry.Start >= start
		&& entry.End is DateTime entryEnd
		&& entryEnd <= end;

	public static DayOfWork Add(this DayOfWork day, Entry entry)
		=> new DayOfWork(
			Start: (day.Start > entry.Start)? entry.Start : day.Start,
			End: (entry.End is null || day.End > entry.End)? day.End : entry.End.Value,
			TimeWorked: day.TimeWorked + ((entry.End?? DateTime.MaxValue) - entry.Start),
			NumberOfBreaks: (entry.Start - day.End) != TimeSpan.Zero? day.NumberOfBreaks + 1 : day.NumberOfBreaks);

	public static TimeSpan GetOvertime(this DayOfWork day)
	{
		var overtime = day.TimeWorked - TimeSpan.FromHours(8);

		if(overtime < TimeSpan.Zero)
			return TimeSpan.Zero;
		
		return overtime;
	}

	public static TimeSpan GetBreakTime(this DayOfWork day)
		=> (day.End - day.Start) - day.TimeWorked;

	public static string FormatForDisplay(this TimeSpan timeSpan)
	{
		double hours = Math.Floor(timeSpan.TotalHours);
		return $"{hours}:{timeSpan.Minutes:D2}";
	}

	public static string FormatDateForDisplay(this DateTime date)
		=> date.ToString("yyyy/MM/dd");
}
