using ErrorHandling;
using TagCloud.Abstractions.Generic;

namespace TagCloud.Implementations.Generics;

public class SourceFactory<TSource>(
    IEnumerable<IWordsProvider<TSource>> providers)
    : ISourceFactory<TSource>
{
    public Result<TSource> Create(string filePath)
    {
        var provider = providers.FirstOrDefault(p => p.CanHandle(filePath));

        return provider == null
            ? Result.Fail<TSource>($"Неподдерживаемый формат файла: {filePath}")
            : Result.Of(
                () => provider.Create(filePath),
                $"Ошибка при создании источника из файла: {filePath}");
    }
}