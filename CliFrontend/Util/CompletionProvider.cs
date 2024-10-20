namespace CliFrontend.Util;

using System.Linq;

public class CompletionProvider
{
	private record Occurence(string Text, int Priority);

	private List<Occurence> _occurences;

	public CompletionProvider(List<string> existingWords)
	{
		Dictionary<string, int> occurrences = new();
		foreach(string word in existingWords)
		{
			if (occurrences.ContainsKey(word))
				occurrences[word]++;
			else
				occurrences[word] = 1;
		}

		_occurences = occurrences
			.Select(entry => new Occurence(entry.Key, entry.Value))
			.ToList();
	}

	public string? GetCompletion(string partialString)
	=> _occurences
			.Where(occurence => occurence.Text.StartsWith(partialString))
			.OrderByDescending(occurence => occurence.Priority)
			.FirstOrDefault()
			?.Text;

}
