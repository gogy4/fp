namespace TagCloud.Abstractions.Generic;

public interface ISourceFactory<TSource>
{
    TSource Create(string filePath);
}