using TagCloud.Abstractions.Generic;
using TagCloud.Abstractions.WordsSource;

namespace TagCloud.Implementations.StopWords;

public class DocxStopWordsProvider(string filePath)
    : DocxFileProviderBase<IWordsSource>(filePath), IWordsSource
{
    public IEnumerable<string> ReadWords()
    {
        return ReadParagraphs();
    }
}