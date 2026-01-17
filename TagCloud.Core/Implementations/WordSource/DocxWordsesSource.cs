using TagCloud.Abstractions.Generic;
using TagCloud.Abstractions.WordsSource;

namespace TagCloud.Implementations.WordSource;

public class DocxWordsSource(string filePath) : DocxFileProviderBase<IWordsSource>(filePath), IWordsSource
{
    public IEnumerable<string> ReadWords()
    {
        return ReadParagraphs();
    }
}