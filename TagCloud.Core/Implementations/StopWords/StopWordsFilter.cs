using Autofac.Features.AttributeFilters;
using TagCloud.Abstractions;
using TagCloud.Abstractions.WordsSource;

namespace TagCloud.Implementations;

public class StopWordsFilter([KeyFilter("stopWords")] IWordsSource provider) : IWordFilter
{
    private readonly IEnumerable<string> stopWords = provider.ReadWords();

    public bool ShouldKeep(string word)
    {
        return !string.IsNullOrWhiteSpace(word) && !stopWords.Contains(word);
    }

    public IEnumerable<string> Filter(IEnumerable<string> words)
    {
        return words.Where(ShouldKeep);
    }
}