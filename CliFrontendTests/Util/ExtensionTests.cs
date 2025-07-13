namespace CliFrontendTests.Util;

using CliFrontend.Util;
using CliFrontendTests.TestUtils;

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
		TestDataGenerator data = new();

		yield return new object [] { data.GetTime(1, 15, 0), data.GetTime(1, 15, 0) };
		yield return new object [] { data.GetTime(1, 22, 0), data.GetTime(1, 15, 0) };
		yield return new object [] { data.GetTime(1, 23, 0), data.GetTime(1, 30, 0) };
		yield return new object [] { data.GetTime(1, 0, 0), data.GetTime(1, 0, 0) };
		yield return new object [] { data.GetTime(1, 7, 0), data.GetTime(1, 0, 0) };
		yield return new object [] { data.GetTime(1, 53, 0), data.GetTime(2, 0, 0) };
	}

}

