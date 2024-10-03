using CliFrontend.Data;
using CliFrontend.IO;
using CliFrontend.Util;

Console.WriteLine("Time Tracking");

if (args.Length < 1)
	throw new ArgumentException("Please provide path to CSV file");

string filePath = args[0];

bool fileFound = File.Exists(filePath);

IList<Entry> previousEntries;
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
	EntryData data = QueryEntryData();
	Entry entry = new(DateTime.UtcNow.RoundToQuarterHour(), null, data);

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

static EntryData QueryEntryData()
{
	string project = QueryValue("Project", "NA");
	string ticket = QueryValue("Ticket", "NA");
	string comment = QueryValue("Comment", "");

	return new EntryData(project, ticket, comment);
}

static string QueryValue(string queryString, string defaultValue = "")
{
	Console.Write($"{queryString}: ");
	string? value = Console.ReadLine();

	if (value?.Trim() is null or "")
	{
		value = defaultValue;
	}

	return value;
}

