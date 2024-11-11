namespace CliFrontend.IO;

using CliFrontend.Data;
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

	public void ShowOverview(List<Entry> entries)
	{
		const string separator = " | ";
		const string unknownValue = "???";

		(int start, int end, int project, int ticket, int comment) columnWidths = entries
			.Select(entry => (
				start: entry.Start.ToString().Length,
				end: entry.End?.ToString().Length ?? unknownValue.Length,
				project: entry.Data.ProjectName.ToString().Length,
				ticket: entry.Data.Ticket.ToString().Length,
				comment: entry.Data.Comment.ToString().Length))
			.Aggregate(
				seed: (start: 0, end: 0, project: 0, ticket: 0, comment: 0),
				(max, current) => (
					start: Math.Max(max.start, current.start),
					end: Math.Max(max.end, current.end),
					project: Math.Max(max.project, current.project),
					ticket: Math.Max(max.ticket, current.ticket),
					comment: Math.Max(max.comment, current.comment)));

		PrintHeader();

		PrintLine(columnWidths.start
				+ columnWidths.end
				+ columnWidths.project
				+ columnWidths.ticket
				+ columnWidths.comment
				+ 4 * separator.Length); // we have 5 entries, so 4 separators

		entries.ForEach(Print);

		return;

		void PrintHeader()
		{
			const string startKeyword = "START";
			const string endKeyword = "END";
			const string projectKeyword = "PROJECT";
			const string ticketKeyword = "TICKET";
			const string commentKeyword = "COMMENT";

			columnWidths.start = Math.Max(columnWidths.start, startKeyword.Length);
			columnWidths.end = Math.Max(columnWidths.end, endKeyword.Length);
			columnWidths.project = Math.Max(columnWidths.project, projectKeyword.Length);
			columnWidths.ticket = Math.Max(columnWidths.ticket, ticketKeyword.Length);
			columnWidths.comment = Math.Max(columnWidths.comment, commentKeyword.Length);

			List<string> captions = new()
			{
				FillToLength(startKeyword, columnWidths.start),
				FillToLength(endKeyword, columnWidths.end),
				FillToLength(projectKeyword, columnWidths.project),
				FillToLength(ticketKeyword, columnWidths.ticket),
				FillToLength(commentKeyword, columnWidths.comment)
			};

			Console.WriteLine(string.Join(separator, captions));
		}

		void Print(Entry entry)
		{
			List<string> entryData = new()
			{
				FillToLength(entry.Start.ToString(), columnWidths.start),
				FillToLength(entry.End?.ToString() ?? unknownValue, columnWidths.end),
				FillToLength(entry.Data.ProjectName, columnWidths.project),
				FillToLength(entry.Data.Ticket, columnWidths.ticket),
				FillToLength(entry.Data.Comment, columnWidths.comment)
			};
			Console.WriteLine(string.Join(separator, entryData));
		}

		static string FillToLength(string input, int desiredLength)
		{
			if (input.Length >= desiredLength)
				return input;

			const char spacer = ' ';

			int bufferSizeBefore = (desiredLength - input.Length) / 2;
			int bufferSizeAfter = desiredLength - input.Length - bufferSizeBefore;
			string bufferBefore = new string(spacer, bufferSizeBefore);
			string bufferAfter = new string(spacer, bufferSizeAfter);

			return bufferBefore + input + bufferAfter;
		}

		static void PrintLine(int length)
		{
			Console.WriteLine(new string('-', length));
		}

	}
}
