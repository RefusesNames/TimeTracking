namespace CliFrontend.IO;

using CliFrontend.Util;

internal class CommandLineService
{
	public string QueryValue(string queryString, string defaultValue)
	{
		Console.Write($"{queryString}: ");
		string? value = Console.ReadLine();

		if (value?.Trim() is null or "")
		{
			value = defaultValue;
		}

		return value;
	}

	public string QueryValue(string queryString, string defaultValue, SuggestionProvider suggestions)
	{
		string displayedText = $"{queryString}: ";
		Console.Write(displayedText);

		string input = string.Empty;
		string currentSuggestion = string.Empty;

		while(true)
		{
			var key = Console.ReadKey(intercept: true);
			switch (key.Key)
			{
				case ConsoleKey.Backspace:
					if (input.Length > 0)
					{
						input = input.Substring(0, input.Length - 1);
						Console.Write("\b \b");
					}
					break;

				case ConsoleKey.Escape:
				{
					int lengthDifference = currentSuggestion.Length - input.Length;
					for (int i = 0; i < lengthDifference; ++i)
						Console.Write(" ");
					for (int i = 0; i < lengthDifference; ++i)
						Console.Write("\b");
					currentSuggestion = "";
					break;
				}


				case ConsoleKey.Enter:
					if (currentSuggestion is not null && currentSuggestion != "")
						input = currentSuggestion;
					Console.WriteLine();
					goto end_of_loop;

				case ConsoleKey.Tab:
					if (currentSuggestion is null || currentSuggestion == "")
						break;
					string charactersToAdd = currentSuggestion.Substring(input.Length);
					Console.Write(charactersToAdd);
					input = currentSuggestion;
					break;

				default:
					input += key.KeyChar;

					if (!currentSuggestion.StartsWith(input))
					{
						int lengthDifference = currentSuggestion.Length - input.Length + 1;
						for (int i = 0; i < lengthDifference; ++i)
							Console.Write(" ");
						for (int i = 0; i < lengthDifference; ++i)
							Console.Write("\b");
						currentSuggestion = "";
					}

					if (suggestions.GetSuggestion(input) is string suggestion)
					{
						currentSuggestion = suggestion;
						Console.Write(suggestion.Substring(input.Length-1));
						int lengthDifference = suggestion.Length - input.Length;
						for (int i = 0; i < lengthDifference; ++i)
							Console.Write("\b");
					}
					else
						Console.Write(key.KeyChar);

					break;
			}
		}
end_of_loop:

		if (input.Trim() is "")
		{
			input = defaultValue;
		}

		return input;
	}
}
