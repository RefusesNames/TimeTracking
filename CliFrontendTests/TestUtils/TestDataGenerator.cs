namespace CliFrontendTests.TestUtils;

using CliFrontend.Data;

internal class TestDataGenerator
{
	private readonly EntryData DefaultEntryData = new("Foo", "Foo-1", "Bar");
	private Entry? lastEntry = null;

	public Entry FirstEntry
	{
		get
		{
			lastEntry = new Entry(
				Start: GetTime(11, 0, 0),
				End: GetTime(12, 0, 0),
				Data: DefaultEntryData);
			return lastEntry;
		}
	}

	public Entry NextEntryWithoutGap
	{
		get
		{
			if (lastEntry is null)
				throw new InvalidOperationException($"Missing call to '{nameof(FirstEntry)}'");

			if (lastEntry.End is null)
				throw new InvalidOperationException($"The last {nameof(Entry)} was invalid, {nameof(Entry.End)} cannot be null");

			lastEntry = new(
				Start: lastEntry.End.Value,
				End: lastEntry.End + TimeSpan.FromMinutes(15),
				Data: DefaultEntryData);

			return lastEntry;
		}
	}

	public Entry NextEntryWithGap
	{
		get
		{
			if (lastEntry is null)
				throw new InvalidOperationException($"Missing call to '{nameof(FirstEntry)}'");

			if (lastEntry.End is null)
				throw new InvalidOperationException($"The last {nameof(Entry)} was invalid, {nameof(Entry.End)} cannot be null");

			DateTime nextStart = lastEntry.End.Value + TimeSpan.FromMinutes(30);

			lastEntry = new(
				Start: nextStart,
				End: nextStart + TimeSpan.FromMinutes(15),
				Data: DefaultEntryData);

			return lastEntry;
		}
	}

	public Entry NextEntryNextDay
	{
		get
		{
			if (lastEntry is null)
				throw new InvalidOperationException($"Missing call to '{nameof(FirstEntry)}'");

			if (lastEntry.End is null)
				throw new InvalidOperationException($"The last {nameof(Entry)} was invalid, {nameof(Entry.End)} cannot be null");

			DateTime nextStart = GetNextDayNoon(lastEntry.End.Value);

			lastEntry = new(
				Start: nextStart,
				End: nextStart + TimeSpan.FromMinutes(15),
				Data: DefaultEntryData);

			return lastEntry;
		}
	}

	public DateTime GetTime(int hours, int minutes, int seconds)
		=> new DateTime(1, 1, 1, hours, minutes, seconds);

	public DateTime GetNextDayNoon(DateTime input)
	{
			DateTime nextDay = input.AddDays(1);
			DateTime nextDayNoon = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 12, 0, 0);
			return nextDayNoon;
	}
}
