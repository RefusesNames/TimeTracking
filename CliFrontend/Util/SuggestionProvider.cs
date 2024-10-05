namespace CliFrontend.Util;

using System.Linq;


public class SuggestionProvider
{
	private record Suggestion(string Text, int Priority);

	private List<Suggestion> _suggestions;

	public SuggestionProvider(List<string> existingWords)
	{
		Dictionary<string, int> occurrences = new();
		foreach(string word in existingWords)
		{
			if (occurrences.ContainsKey(word))
				occurrences[word]++;
			else
				occurrences[word] = 1;
		}

		_suggestions = occurrences
			.Select(entry => new Suggestion(entry.Key, entry.Value))
			.ToList();
	}

	public string? GetSuggestion(string partialString)
	=> _suggestions
			.Where(suggestion => suggestion.Text.StartsWith(partialString))
			.OrderByDescending(suggestion => suggestion.Priority)
			.FirstOrDefault()
			?.Text;

}
