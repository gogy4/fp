using TagCloud.Abstractions;

namespace TagCloud.Implementations.WordSource;

public class LowercasePreprocessor : IWordPreprocessor
{
    public string Preprocess(string word)
    {
        return word.ToLowerInvariant();
    }

    public IEnumerable<string> PreprocessMany(IEnumerable<string> words)
    {
        foreach (var w in words)
            if (!string.IsNullOrWhiteSpace(w))
                yield return Preprocess(w);
    }
}