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

	public string QueryValue(string queryString, string defaultValue, CompletionProvider completions)
	{
		string displayedText = $"{queryString}: ";
		Console.Write(displayedText);

		string input = string.Empty;
		string currentCompletion = string.Empty;

		while(true)
		{
			var key = Console.ReadKey(intercept: true);
			switch (key.Key)
			{
				case ConsoleKey.Backspace:
					if (input.Length > 0)
					{
						input = input.Substring(0, input.Length - 1);
						ClearLastCharacter();
					}
					break;

				case ConsoleKey.Escape:
				{
					int lengthDifference = currentCompletion!.Length - input.Length;
					for (int i = 0; i < lengthDifference; ++i)
						Console.Write(" ");
					for (int i = 0; i < lengthDifference; ++i)
						Console.Write("\b");
					currentCompletion = "";
					break;
				}


				case ConsoleKey.Enter:
					if (currentCompletion is not null && currentCompletion != "")
						input = currentCompletion;
					Console.WriteLine();
					goto end_of_loop;

				case ConsoleKey.Tab:
					if (currentCompletion is null || currentCompletion == "")
						break;
					string charactersToAdd = currentCompletion.Substring(input.Length);
					Console.Write(charactersToAdd);
					input = currentCompletion;
					break;

				default:
					input += key.KeyChar;

					if (!currentCompletion!.StartsWith(input))
					{
						int lengthDifference = currentCompletion.Length - input.Length + 1;
						for (int i = 0; i < lengthDifference; ++i)
							Console.Write(" ");
						for (int i = 0; i < lengthDifference; ++i)
							Console.Write("\b");
						currentCompletion = "";
					}

					if (completions.GetCompletion(input) is string completion)
					{
						currentCompletion = completion;
						Console.Write(completion.Substring(input.Length-1));
						int lengthDifference = completion.Length - input.Length;
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

	public string QueryValue(string queryString, string defaultValue, SuggestionProvider suggestions)
	{
		string displayedText = $"{queryString}: ";
		Console.Write(displayedText);

		string suggestion = suggestions.GetSuggestion();
		Console.Write(suggestion);

		string input = suggestion;

		while(true)
		{
			var key = Console.ReadKey(intercept: true);
			switch (key.Key)
			{
				case ConsoleKey.Backspace:
					if (input.Length > 0)
					{
						input = input.Substring(0, input.Length - 1);
						ClearLastCharacter();
					}
					break;

				case ConsoleKey.Escape:
				{
					ClearLastCharacters(input.Length);
					input = "";
					break;
				}

				case ConsoleKey.Enter:
					Console.WriteLine();
					goto end_of_loop;

				default:
					input += key.KeyChar;
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

	private static void ClearLastCharacter()
		=> Console.Write("\b \b");

	private static void ClearLastCharacters(int numOfCharacters)
	{
		for (int i = 0; i < numOfCharacters; i++)
		{
			ClearLastCharacter();
		}
	}
}
