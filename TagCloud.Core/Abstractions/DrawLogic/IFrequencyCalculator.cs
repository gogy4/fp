namespace TagCloud.Abstractions;

public interface IFrequencyCalculator
{
    IReadOnlyDictionary<string, int> Calculate(IEnumerable<string> words);
}