
namespace CliFrontend.Data;

public record DayOfWork(
		DateTime Start,
		DateTime End,
		TimeSpan TimeWorked,
		int NumberOfBreaks);
