namespace TagCloud.Abstractions.Generic;

public interface IWordsProvider<TSource>
{
    bool CanHandle(string filePath);
    TSource Create(string filePath);
}