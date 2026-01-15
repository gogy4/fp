using TagCloud.Abstractions.Generic;

namespace TagCloud.Implementations.Generics;

public class SourceFactory<TSource>(IEnumerable<IWordsProvider<TSource>> providers) : ISourceFactory<TSource>
{
    public TSource Create(string filePath)
    {
        var provider = providers.FirstOrDefault(p => p.CanHandle(filePath));

        return provider == null
            ? throw new NotSupportedException($"Unsupported format: {filePath}")
            : provider.Create(filePath);
    }
}