namespace CliFrontend.Util;

public interface ITimeProvider
{
	DateTime UtcNow { get; }
	DateTime Today { get; }
}

public class TimeProvider : ITimeProvider
{
	public DateTime UtcNow => DateTime.UtcNow;
	public DateTime Today => DateTime.Today;
}
