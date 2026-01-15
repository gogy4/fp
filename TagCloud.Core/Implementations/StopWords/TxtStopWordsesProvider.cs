using TagCloud.Abstractions.Generic;
using TagCloud.Abstractions.WordsSource;

namespace TagCloud.Implementations;

public class TxtStopWordsProvider(string filePath)
    : TxtFileProviderBase(filePath), IWordsSource
{
    public IEnumerable<string> ReadWords()
    {
        return ReadLines().ToHashSet();
    }
}