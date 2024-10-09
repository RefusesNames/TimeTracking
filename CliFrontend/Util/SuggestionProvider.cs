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

	public string GetCommonPrefix()
		=> _suggestions
		.Select(suggestion => suggestion.Text)
		.Aggregate(string.Empty, (word1, word2) => GetCommonPrefix(word1, word2));

	private static string GetCommonPrefix(string word1, string word2)
	{
		string currentLongestPrefix = string.Empty;
		for (int i = 1; i < word1.Length; ++i)
		{
			string prefixCandidate = word1.Substring(0, i);
			if (word2.StartsWith(prefixCandidate))
				currentLongestPrefix = prefixCandidate;
			else
				break;
		}

		return currentLongestPrefix;
	}

}
