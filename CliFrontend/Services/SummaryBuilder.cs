using CliFrontend.Data;
using CliFrontend.Util;

namespace CliFrontend.Services;

public class SummaryBuilder(List<Entry> entries)
{

	public List<DayOfWork> GetDaysOfWork()
		=> GetDaysOfWork(
			from: DateTime.MinValue,
			until: DateTime.Now);

	public List<DayOfWork> GetDaysOfWork(DateTime from, DateTime until)
		=> entries
			.Where(entry => entry.IsBetween(from, until))
			.GroupBy(entry => entry.Start.Date)
			.Select(SummarizeEntriesToDay)
			.ToList();

	private static DayOfWork SummarizeEntriesToDay(IEnumerable<Entry> entries)
		=> entries
			.Aggregate<Entry, DayOfWork>(
				new DayOfWork(Start: DateTime.MaxValue, End: DateTime.MinValue, TimeSpan.Zero, -1),
				(dayOfWork, entry) => dayOfWork.Add(entry));
}
