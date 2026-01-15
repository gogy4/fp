namespace TagCloud.Abstractions.Generic;

public abstract class TxtProviderBase<T> : IWordsProvider<T>
{
    private readonly string extension;

    protected TxtProviderBase(string extension)
    {
        this.extension = extension.StartsWith(".") ? extension : "." + extension;
    }

    public bool CanHandle(string filePath)
    {
        return Path.GetExtension(filePath).Equals(extension, StringComparison.OrdinalIgnoreCase);
    }

    public T Create(string filePath)
    {
        return CreateProvider(filePath);
    }

    protected abstract T CreateProvider(string filePath);
}