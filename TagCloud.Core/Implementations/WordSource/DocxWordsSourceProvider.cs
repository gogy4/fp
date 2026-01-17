using TagCloud.Abstractions.Generic;
using TagCloud.Abstractions.WordsSource;

namespace TagCloud.Implementations.WordSource;

public class DocxWordsSourceProvider() : TxtProviderBase<IWordsSource>(".docx")
{
    protected override IWordsSource CreateProvider(string filePath)
    {
        return new DocxWordsSource(filePath);
    }
}