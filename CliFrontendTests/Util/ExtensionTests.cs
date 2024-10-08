namespace CliFrontendTests.Util;

using CliFrontend.Util;

public class ExtensionTests
{
	[Theory]
	[MemberData(nameof(GetTimes))]
	public void RoundToQuarterHour_Rounds_Correctly(DateTime originalTime, DateTime expectedTime)
	{
		var rounded = originalTime.RoundToQuarterHour();

		Assert.Equal(expectedTime, rounded);
	}

	public static IEnumerable<object[]> GetTimes()
	{
		yield return new object [] { ByTime(1, 15, 0), ByTime(1, 15, 0) };
		yield return new object [] { ByTime(1, 22, 0), ByTime(1, 15, 0) };
		yield return new object [] { ByTime(1, 23, 0), ByTime(1, 30, 0) };
		yield return new object [] { ByTime(1, 0, 0), ByTime(1, 0, 0) };
		yield return new object [] { ByTime(1, 7, 0), ByTime(1, 0, 0) };
		yield return new object [] { ByTime(1, 53, 0), ByTime(2, 0, 0) };

		static DateTime ByTime(int hours, int minutes, int seconds)
			=> new DateTime(1, 1, 1, hours, minutes, seconds);
	}

}

