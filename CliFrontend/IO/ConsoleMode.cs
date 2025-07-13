namespace CliFrontend.IO;

public sealed class ConsoleMode : IDisposable
{
	public static ConsoleMode Warning
		=> new(
				foregroundColor: ConsoleColor.White,
				backgroundColor: ConsoleColor.DarkYellow);

	public static ConsoleMode Error
		=> new(
				foregroundColor: ConsoleColor.White,
				backgroundColor: ConsoleColor.DarkRed);

	private ConsoleMode(
		ConsoleColor foregroundColor,
		ConsoleColor backgroundColor)
	{
		Console.ForegroundColor = foregroundColor;
		Console.BackgroundColor = backgroundColor;
	}

	public void Dispose()
	{
		Console.ResetColor();
	}
}

