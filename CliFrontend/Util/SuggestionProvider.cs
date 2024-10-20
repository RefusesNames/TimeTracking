namespace CliFrontend.Util;

using System.Linq;

public class SuggestionProvider
{
	private record Occurence(string Text, int Priority);

	private List<Occurence> _occurences;

	public SuggestionProvider(List<string> existingWords, List<string> wordsToIgnore)
	{
		Dictionary<string, int> occurrences = new();
		foreach(string word in existingWords)
		{
			if (wordsToIgnore.Contains(word))
				continue;

			if (occurrences.ContainsKey(word))
				occurrences[word]++;
			else
				occurrences[word] = 1;
		}

		_occurences = occurrences
			.Select(entry => new Occurence(entry.Key, entry.Value))
			.ToList();
	}

	public string GetSuggestion()
		=> _occurences
		.Select(suggestion => suggestion.Text)
		.Aggregate(FirstOccurenceTextOrEmpty, GetCommonPrefix);

	private string FirstOccurenceTextOrEmpty
		=> _occurences.FirstOrDefault()?.Text ?? "";

	private static string GetCommonPrefix(string word1, string word2)
	{
		string currentLongestPrefix = string.Empty;
		for (int i = 1; i <= word1.Length; ++i)
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
