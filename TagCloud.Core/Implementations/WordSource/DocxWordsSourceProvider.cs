using TagCloud.Abstractions.Generic;
using TagCloud.Abstractions.WordsSource;

namespace TagCloud.Implementations;

public class DocxWordsSourceProvider() : TxtProviderBase<IWordsSource>(".docx")
{
    protected override IWordsSource CreateProvider(string filePath)
    {
        return new DocxWordsSource(filePath);
    }
}