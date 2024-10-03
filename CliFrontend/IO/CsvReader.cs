namespace CliFrontend.IO;

using CliFrontend.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CsvReader(StreamReader reader)
{
	private Regex _lineRegex
		= new(@"(?<Start>[^,]+),(?<End>[^,]*),(?<Project>[^,]+),""(?<Ticket>[^""]+)"",""(?<Comment>[^""]*)""");

	public IList<Entry> Read()
	{
		string? header = reader.ReadLine();
		AssertHeaderIsValid(header);

		List<Entry> entries = new();
		while(reader.ReadLine() is string line)
		{
			entries.Add(ToEntry(line));
		}

		return entries;
	}

	private void AssertHeaderIsValid(string? header)
	{
		if (header != "Start,End,Project,Ticket,Comment")
			throw new Exception($"Invalid file, header was \"{header}\"");
	}

	private Entry ToEntry(string line)
	{
		var match = _lineRegex.Match(line);
		if (!match.Success)
			throw new Exception($"Invalid entry: {line}");

		var start = Parse<DateTime>(match.Groups["Start"].Value);
		var end = ParseNullable<DateTime>(match.Groups["End"].Value);
		var project = match.Groups["Project"].Value;
		var ticket = match.Groups["Ticket"].Value;
		var comment = match.Groups["Comment"].Value;

		return new Entry(start, end, new EntryData(project, ticket, comment));
	}

	private T Parse<T>(string text) where T : IParsable<T>
	{
		if (!T.TryParse(text, null, out T? parsedObject))
			throw new Exception($"Failed to parse {text} as {typeof(T)}");

		return parsedObject;
	}

	private T? ParseNullable<T>(string text) where T : struct, IParsable<T>
	{
		if (text == string.Empty)
			return null;

		if (!T.TryParse(text, null, out T parsedObject))
			throw new Exception($"Failed to parse {text} as {typeof(T)}");

		return parsedObject;
	}
}

