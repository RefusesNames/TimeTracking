using CliFrontend.Data;
using CliFrontend.IO;
using CliFrontend.Util;

const string DefaultProject = "NA";
const string DefaultTicket = "NA";

Console.WriteLine("Time Tracking");

if (args.Length < 1)
	throw new ArgumentException("Please provide path to CSV file");

string filePath = args[0];

bool fileFound = File.Exists(filePath);

List<Entry> previousEntries;
if (fileFound)
{
	using StreamReader streamReader = new(args[0]);
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


