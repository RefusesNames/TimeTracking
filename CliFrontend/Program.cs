﻿using CliFrontend.Data;
using CliFrontend.IO;
using CliFrontend.Services;
using CliFrontend.Util;
using System.CommandLine;
using TimeProvider = CliFrontend.Util.TimeProvider;

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

// TODO: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage

Command trackCommand = new(
	"track",
	description: "Add a new or finish the previous tracking entry");

ITimeProvider timeProvider = new TimeProvider();

trackCommand.AddArgument(pathArgument);
trackCommand.SetHandler(filePath => Track(filePath, timeProvider), pathArgument);

Argument<int> numberOfMonthsArgument = new(
	"months",
	description: "Number of months to evaluate")
{
	Arity = ArgumentArity.ZeroOrOne
};

Command evaluateCommand = new(
	"eval",
	description: "Evaluate the given time frame");
evaluateCommand.AddArgument(pathArgument);
evaluateCommand.AddArgument(numberOfMonthsArgument);
evaluateCommand.SetHandler(
	(filePath, numberOfMonths)	=> Evaluate(filePath, numberOfMonths, timeProvider),
	pathArgument,
	numberOfMonthsArgument);

Command listCommand = new(
	"list",
	description: "Lists all data");
listCommand.AddArgument(pathArgument);
listCommand.SetHandler(List, pathArgument);

Command checkCommand = new(
	"check",
	description: "Checks the data for unusual stuff");
checkCommand.AddArgument(pathArgument);
checkCommand.SetHandler(Check, pathArgument);

rootCommand.AddCommand(trackCommand);
rootCommand.AddCommand(evaluateCommand);
rootCommand.AddCommand(listCommand);
rootCommand.AddCommand(checkCommand);


return await rootCommand.InvokeAsync(args);


static void Track(string filePath, ITimeProvider timeProvider)
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
				timeProvider.UtcNow.RoundToQuarterHour(),
				lastEntry.Data);
		TimeSpan timeTracked = previousEntries.Last().End - previousEntries.Last().Start
			?? TimeSpan.Zero;
		var trackedFor = previousEntries.Last()!.Data;
		Console.WriteLine("Tracked {0} hours, {1} minutes for '{2}' ({3})",
				timeTracked.Hours,
				timeTracked.Minutes,
				trackedFor.Ticket,
				trackedFor.ProjectName);
		using StreamWriter streamWriter = new(filePath, append: false);
		CsvWriter csvWriter = new CsvWriter(streamWriter);
		csvWriter.WriteHeader();
		foreach(Entry entry in previousEntries)
			csvWriter.Write(entry);
	}
	else
	{
		EntryData data = QueryEntryData(previousEntries);

		var timeNow = timeProvider.UtcNow.RoundToQuarterHour();
		Entry entry;

		if (previousEntries.Count > 0
			&& (timeNow - previousEntries.Last().End!.Value) < TimeSpan.FromMinutes(20))
			entry = new(previousEntries.Last().End!.Value, null, data); // fill gaps that occur while entering data
		else
			entry = new(timeProvider.UtcNow.RoundToQuarterHour(), null, data);

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

static void Evaluate(string filePath, int numberOfMonths, ITimeProvider timeProvider)
{
	List<Entry> entries = LoadFromFile(filePath);

	if (numberOfMonths == 0)
	{
		Console.WriteLine("TOTAL TIME:");
		Console.WriteLine("\tToday: {0}", entries.GetTimeTrackedToday(timeProvider).FormatForDisplay());
		Console.WriteLine("\tThis month: {0}", entries.GetTimeTrackedThisMonth(timeProvider).FormatForDisplay());
		Console.WriteLine("\tLast month: {0}", entries.GetTimeTrackedLastMonth(timeProvider).FormatForDisplay());

		Console.WriteLine("\nDays worked this month: {0}", entries.GetDaysWorkedThisMonth(timeProvider));
		Console.WriteLine("\nDays worked last month: {0}", entries.GetDaysWorkedLastMonth(timeProvider));
		Console.WriteLine("\nAccumulated overtime: {0}", entries.GetOvertime().FormatForDisplay());

		Console.WriteLine("\nBY PROJECT:");
		List<IGrouping<string, Entry>> entriesByProject = entries
			.GroupBy(entry => entry.Data.ProjectName)
			.ToList();

		foreach(IGrouping<string, Entry> projectGroup in entriesByProject)
		{
			string projectName = projectGroup.Key;
			TimeSpan trackedToday = projectGroup.GetTimeTrackedToday(timeProvider);
			TimeSpan trackedThisMonth = projectGroup.GetTimeTrackedThisMonth(timeProvider);
			TimeSpan trackedLastMonth = projectGroup.GetTimeTrackedLastMonth(timeProvider);

			Console.WriteLine("\t- {0}", projectName);
			Console.WriteLine("\t\tToday: {0}", trackedToday.FormatForDisplay());
			Console.WriteLine("\t\tThis month: {0}", trackedThisMonth.FormatForDisplay());
			Console.WriteLine("\t\tLast month: {0}", trackedLastMonth.FormatForDisplay());
		}
	}
	else if (numberOfMonths > 0)
	{
		IEnumerable<DateTime> months = timeProvider.GetLastNMonths(numberOfMonths);
		foreach(DateTime month in months)
		{
			List<Entry> entriesOfTheMonth = entries
				.Where(entry => entry.HasEndTime())
				.Where(entry => entry.Start.IsSameMonthAs(month))
				.ToList();

			Console.WriteLine($"\n{month.Month}.{month.Year}:");
			Console.WriteLine($"Time tracked: {entriesOfTheMonth.GetTimeTracked().FormatForDisplay()}");
			Console.WriteLine($"Days worked: {entriesOfTheMonth.GetDaysWorked()}");
			List<IGrouping<string, Entry>> entriesByProject = entriesOfTheMonth
				.GroupBy(entry => entry.Data.ProjectName)
				.ToList();
			foreach(IGrouping<string, Entry> projectGroup in entriesByProject)
			{
				string projectName = projectGroup.Key;
				TimeSpan timeTracked = projectGroup.GetTimeTracked();

				Console.WriteLine("For {0}: {1}", projectName, timeTracked.FormatForDisplay());
			}
		}
	}
	else if (numberOfMonths < 0)
	{
		throw new ArgumentException("Cannot retrieve the entries for a negative amount of months");
	}
}

static void List(string filePath)
{
	List<Entry> entries = LoadFromFile(filePath);

	CommandLineService commandLine = new();
	commandLine.ShowOverview(entries);
}

static void Check(string filePath)
{
	List<Entry> entries = LoadFromFile(filePath);
	SummaryBuilder summaryBuilder = new(entries);
	List<DayOfWork> days = summaryBuilder.GetDaysOfWork();

	CommandLineService.ShowCheckResults(days);
}


static List<Entry> LoadFromFile(string filePath)
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

	return entries;
}
