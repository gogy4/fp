using TagCloud.Abstractions.Generic;
using TagCloud.Abstractions.WordsSource;

namespace TagCloud.Implementations.StopWords;

public class TxtStopWordsTxtProvider() : TxtProviderBase<IWordsSource>(".txt")
{
    protected override IWordsSource CreateProvider(string filePath)
    {
        return new TxtStopWordsProvider(filePath);
    }
}