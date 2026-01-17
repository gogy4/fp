using TagCloud.Abstractions.Generic;
using TagCloud.Abstractions.WordsSource;

namespace TagCloud.Implementations.WordSource;

public class TxtWordsSourceProvider :
    IWordsProvider<IWordsSource>
{
    public bool CanHandle(string filePath)
    {
        return Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase);
    }

    public IWordsSource Create(string filePath)
    {
        return new TxtWordsSource(filePath);
    }
}