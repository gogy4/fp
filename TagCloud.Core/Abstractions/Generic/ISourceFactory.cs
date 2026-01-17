using ErrorHandling;

namespace TagCloud.Abstractions.Generic;

public interface ISourceFactory<TSource>
{
    Result<TSource> Create(string filePath);
}