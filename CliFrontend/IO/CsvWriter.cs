namespace CliFrontend.IO;

using CliFrontend.Data;

public class CsvWriter(StreamWriter writer)
{
	public void WriteHeader()
	{
		string csvString = "Start,End,Project,Ticket,Comment";
		writer.WriteLine(csvString);
	}

	public void Write(Entry entry)
	{
		string csvString = ToCSV(entry);
		writer.WriteLine(csvString);
	}

	private string ToCSV(Entry entry)
		=> $"{entry.Start}"
		+ $",{entry.End}"
		+ $",{entry.Data.ProjectName}"
		+ $",\"{entry.Data.Ticket}\""
		+ $",\"{entry.Data.Comment}\"";
}
