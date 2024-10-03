namespace CliFrontend.Data;

public record EntryData(string ProjectName, string Ticket, string Comment);

public record Entry(DateTime Start, DateTime? End, EntryData Data);
