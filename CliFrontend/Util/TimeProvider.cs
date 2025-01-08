namespace CliFrontend.Util;

public interface ITimeProvider
{
	DateTime UtcNow { get; }
	DateTime Today { get; }
	IEnumerable<DateTime> GetLastNMonths(int numberOfMonths);
}

public class TimeProvider : ITimeProvider
{
	public DateTime UtcNow => DateTime.UtcNow;
	public DateTime Today => DateTime.Today;

	public IEnumerable<DateTime> GetLastNMonths(int numberOfMonths)
	{
		DateTime thisMonth = new(
			year: Today.Year,
			month: Today.Month,
			day: 1);
		for (int i = 0; i < numberOfMonths; i++)
		{
			yield return thisMonth.AddMonths(-1 * i);
		}
	}
}
