namespace TagCloud.Abstractions.Generic;

public abstract class TxtFileProviderBase(string filePath)
{
    private readonly string FilePath = filePath;

    protected IEnumerable<string> ReadLines()
    {
        return File.ReadLines(FilePath)
            .Select(s => s.Trim().ToLowerInvariant())
            .Where(s => !string.IsNullOrEmpty(s));
    }
}