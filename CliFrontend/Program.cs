using CliFrontend.Data;
using CliFrontend.IO;
using CliFrontend.Services;
using CliFrontend.Util;
using System.CommandLine;

const string DefaultProject = "NA";
const string DefaultTicket = "NA";


RootCommand rootCommand
	= new("A simple CLI time tracking tool that also allows evaluation");

Argument<string> pathArgument = new(
	"filePath",
	description: "The path of the file to use as storage")
{
	Arity = ArgumentArity.ExactlyOne
};


Command trackCommand = new(
	"track",
	description: "Add a new or finish the previous tracking entry");

trackCommand.AddArgument(pathArgument);
trackCommand.SetHandler(Track, pathArgument);

Command evaluateCommand = new(
	"eval",
	description: "Evaluate the given time frame");
evaluateCommand.AddArgument(pathArgument);
evaluateCommand.SetHandler(Evaluate, pathArgument);

rootCommand.AddCommand(trackCommand);
rootCommand.AddCommand(evaluateCommand);

return await rootCommand.InvokeAsync(args);


static void Track(string filePath)
{
	bool fileFound = File.Exists(filePath);

	List<Entry> previousEntries;
	if (fileFound)
	{
		using StreamReader streamReader = new(filePath);
		CsvReader csvReader = new(streamReader);
		previousEntries = csvReader.Read();
	}
	else
		previousEntries = new List<Entry>();


	Entry? lastEntry = previousEntries.LastOrDefault();
	if (IsUnfinished(lastEntry))
	{
		previousEntries[^1] = new(
				lastEntry!.Start,
				DateTime.UtcNow.RoundToQuarterHour(),
				lastEntry.Data);
		using StreamWriter streamWriter = new(filePath, append: false);
		CsvWriter csvWriter = new CsvWriter(streamWriter);
		csvWriter.WriteHeader();
		foreach(Entry entry in previousEntries)
			csvWriter.Write(entry);
	}
	else
	{
		EntryData data = QueryEntryData(previousEntries);

		var timeNow = DateTime.UtcNow.RoundToQuarterHour();
		Entry entry;

		if (previousEntries.Count > 0
			&& (timeNow - previousEntries.Last().End!.Value) < TimeSpan.FromMinutes(20))
			entry = new(previousEntries.Last().End!.Value, null, data); // fill gaps that occur while entering data
		else
			entry = new(DateTime.UtcNow.RoundToQuarterHour(), null, data);

		using StreamWriter streamWriter = new(filePath, append: true);
		CsvWriter csvWriter = new CsvWriter(streamWriter);

		if(!fileFound)
		{
			csvWriter.WriteHeader();
		}

		csvWriter.Write(entry);
	}

	return;

	static bool IsUnfinished(Entry? entry)
		=> entry is { End: null };

	static EntryData QueryEntryData(IReadOnlyList<Entry> previousEntries)
	{
		List<string> projects = previousEntries
			.Select(entry => entry.Data.ProjectName)
			.ToList();
		CompletionProvider completionProviderProject = new(projects);

		CommandLineService commandLine = new();

		string project = commandLine.QueryValue("Project", DefaultProject, completionProviderProject);

		List<string> tickets = previousEntries
			.Where(entry => entry.Data.ProjectName == project)
			.Select(entry => entry.Data.Ticket)
			.ToList();
		SuggestionProvider suggestionProviderTicket = new(tickets, new() {DefaultTicket});
		string ticket = commandLine.QueryValue("Ticket", DefaultTicket, suggestionProviderTicket);
		string comment = commandLine.QueryValue("Comment", "");

		return new EntryData(project, ticket, comment);
	}
}

static void Evaluate(string filePath)
{
	bool fileFound = File.Exists(filePath);

	List<Entry> entries;
	if (fileFound)
	{
		using StreamReader streamReader = new(filePath);
		CsvReader csvReader = new(streamReader);
		entries = csvReader.Read();
	}
	else
		throw new ArgumentException($"File not found: {filePath}");

	Console.WriteLine("TOTAL TIME:");
	Console.WriteLine("\tToday: {0}", entries.GetTimeTrackedToday());
	Console.WriteLine("\tThis month: {0}", entries.GetTimeTrackedThisMonth());

	Console.WriteLine("\nDays worked this month: {0}", entries.GetDaysWorked());

	Console.WriteLine("\nBY PROJECT:");
	List<IGrouping<string, Entry>> entriesByProject = entries
		.GroupBy(entry => entry.Data.ProjectName)
		.ToList();

	foreach(IGrouping<string, Entry> projectGroup in entriesByProject)
	{
		string projectName = projectGroup.Key;
		TimeSpan trackedToday = projectGroup.GetTimeTrackedToday();
		TimeSpan trackedThisMonth = projectGroup.GetTimeTrackedThisMonth();


		Console.WriteLine("\t- {0}", projectName);
		Console.WriteLine("\t\tToday: {0}", trackedToday);
		Console.WriteLine("\t\tThis month: {0}", trackedThisMonth);
	}

	return;
}
