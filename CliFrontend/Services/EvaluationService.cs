using CliFrontend.Data;
using CliFrontend.Util;

namespace CliFrontend.Services;

public static class EvaluationService
{
	public static TimeSpan GetTimeTrackedToday(this IEnumerable<Entry> entries)
		=> entries
		.Where(entry => entry.HasEndTime())
		.Where(entry => entry.Start.Date == DateTime.Today)
		.Select(entry => (entry.End - entry.Start) ?? TimeSpan.Zero)
		.Aggregate(
				seed: TimeSpan.Zero,
				(total, nextTimeSpan) => total + nextTimeSpan);

	public static TimeSpan GetTimeTrackedThisMonth(this IEnumerable<Entry> entries)
		=> entries
		.Where(entry => entry.HasEndTime())
		.Where(IsThisMonth)
		.Select(entry => (entry.End - entry.Start) ?? TimeSpan.Zero)
		.Aggregate(
				seed: TimeSpan.Zero,
				(total, nextTimeSpan) => total + nextTimeSpan);

	public static TimeSpan GetTimeTrackedLastMonth(this IEnumerable<Entry> entries)
		=> entries
		.Where(entry => entry.HasEndTime())
		.Where(IsLastMonth)
		.Select(entry => (entry.End - entry.Start) ?? TimeSpan.Zero)
		.Aggregate(
				seed: TimeSpan.Zero,
				(total, nextTimeSpan) => total + nextTimeSpan);

	public static int GetDaysWorked(this IEnumerable<Entry> entries)
		=> entries
		.Where(entry => entry.HasEndTime())
		.Where(IsThisMonth)
		.Select(entry => entry.Start.Date)
		.Distinct()
		.Count();

	public static int GetDaysWorkedLastMonth(this IEnumerable<Entry> entries)
		=> entries
		.Where(entry => entry.HasEndTime())
		.Where(IsThisMonth)
		.Select(entry => entry.Start.Date)
		.Distinct()
		.Count();

	public static TimeSpan GetTimeTracked(this IEnumerable<Entry> entries)
		=> entries
		.Where(entry => entry.HasEndTime())
		.Distinct()
		.Select(entry => (entry.End - entry.Start) ?? TimeSpan.Zero)
		.Aggregate(
				seed: TimeSpan.Zero,
				(total, nextTimeSpan) => total + nextTimeSpan);

	public static TimeSpan GetOvertime(this IEnumerable<Entry> entries)
		=> entries.GetTimeTracked() - entries.GetDaysWorked() * TimeSpan.FromHours(8);

	private static bool IsThisMonth(Entry entry)
		=> entry.Start.Date.Year == DateTime.Today.Year
				&& entry.Start.Date.Month == DateTime.Today.Month;

	private static bool IsLastMonth(Entry entry)
		=> entry.Start.Date.Year == DateTime.Today.Year
				&& entry.Start.Date.Month == GetMonthBefore(DateTime.Today.Month);

	private static int GetMonthBefore(int monthNumber)
	{
		if (monthNumber == 1)
			return 12;

		return monthNumber - 1;
	}
}
