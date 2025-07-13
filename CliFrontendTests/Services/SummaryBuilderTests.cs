namespace CliFrontendTests.Services;

using CliFrontend.Data;
using CliFrontend.Services;
using CliFrontendTests.TestUtils;

public class SummaryBuilderTests
{
	private TestDataGenerator _data = new();

	[Fact]
	public void GetDaysOfWork_Detects_NoBreak_SingleEntry()
	{
		var entries = new List<Entry>{ _data.FirstEntry };
		SummaryBuilder builder = new(entries);

		var days = builder.GetDaysOfWork();
		var numberOfBreaks = SumBreaks(days);
		Assert.Equal(0, numberOfBreaks);
	}

	[Fact]
	public void GetDaysOfWork_Detects_NoBreak_MultipleEntries()
	{
		var entries = new List<Entry>{ _data.FirstEntry, _data.NextEntryWithoutGap };
		SummaryBuilder builder = new(entries);

		var days = builder.GetDaysOfWork();
		var numberOfBreaks = SumBreaks(days);
		Assert.Equal(0, numberOfBreaks);
	}

	[Fact]
	public void GetDaysOfWork_Detects_NoBreak_MultipleDays()
	{
		var entries = new List<Entry>
		{
			_data.FirstEntry, _data.NextEntryWithoutGap,
			_data.NextEntryNextDay, _data.NextEntryWithoutGap,
		};
		SummaryBuilder builder = new(entries);

		var days = builder.GetDaysOfWork();
		var numberOfBreaks = SumBreaks(days);
		Assert.Equal(0, numberOfBreaks);
	}

	[Fact]
	public void GetDaysOfWork_Detects_SingleBreak_TwoEntries()
	{
		var entries = new List<Entry>
		{
			_data.FirstEntry, _data.NextEntryWithGap,
		};
		SummaryBuilder builder = new(entries);

		var days = builder.GetDaysOfWork();
		var numberOfBreaks = SumBreaks(days);
		Assert.Equal(1, numberOfBreaks);
	}

	[Fact]
	public void GetDaysOfWork_Detects_SingleBreak_ThreeEntries()
	{
		var entries = new List<Entry>
		{
			_data.FirstEntry, _data.NextEntryWithGap, _data.NextEntryWithoutGap
		};
		SummaryBuilder builder = new(entries);

		var days = builder.GetDaysOfWork();
		var numberOfBreaks = SumBreaks(days);
		Assert.Equal(1, numberOfBreaks);
	}

	[Fact]
	public void GetDaysOfWork_Detects_SingleBreak_MultipleDays()
	{
		var entries = new List<Entry>
		{
			_data.FirstEntry, _data.NextEntryWithGap,
			_data.NextEntryNextDay
		};
		SummaryBuilder builder = new(entries);

		var days = builder.GetDaysOfWork();
		var numberOfBreaks = SumBreaks(days);
		Assert.Equal(1, numberOfBreaks);
	}

	[Fact]
	public void GetDaysOfWork_Detects_MultipleBreaks_ThreeEntries()
	{
		var entries = new List<Entry>
		{
			_data.FirstEntry, _data.NextEntryWithGap, _data.NextEntryWithGap
		};
		SummaryBuilder builder = new(entries);

		var days = builder.GetDaysOfWork();
		var numberOfBreaks = SumBreaks(days);
		Assert.Equal(2, numberOfBreaks);
	}

	[Fact]
	public void GetDaysOfWork_Detects_MultipleBreaks_FourEntries()
	{
		var entries = new List<Entry>
		{
			_data.FirstEntry, _data.NextEntryWithGap, _data.NextEntryWithGap, _data.NextEntryWithoutGap
		};
		SummaryBuilder builder = new(entries);

		var days = builder.GetDaysOfWork();
		var numberOfBreaks = SumBreaks(days);
		Assert.Equal(2, numberOfBreaks);
	}

	[Fact]
	public void GetDaysOfWork_Detects_MultipleBreaks_MultipleDays()
	{
		var entries = new List<Entry>
		{
			_data.FirstEntry, _data.NextEntryWithGap,
			_data.NextEntryNextDay, _data.NextEntryWithGap
		};
		SummaryBuilder builder = new(entries);

		var days = builder.GetDaysOfWork();
		var numberOfBreaks = SumBreaks(days);
		Assert.Equal(2, numberOfBreaks);
	}

	private static int SumBreaks(List<DayOfWork> days)
		=> days
		.Select(day => day.NumberOfBreaks)
		.Aggregate(0, (sum, next) => sum + next);
}
