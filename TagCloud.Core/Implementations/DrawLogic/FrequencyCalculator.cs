using TagCloud.Abstractions;

namespace TagCloud.Implementations;

public class FrequencyCalculator : IFrequencyCalculator
{
    public IReadOnlyDictionary<string, int> Calculate(IEnumerable<string> words)
    {
        var dict = new Dictionary<string, int>();
        foreach (var w in words) dict[w] = dict.GetValueOrDefault(w) + 1;

        return dict;
    }
}