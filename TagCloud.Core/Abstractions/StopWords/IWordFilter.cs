namespace TagCloud.Abstractions;

public interface IWordFilter
{
    bool ShouldKeep(string preprocessedWord);
    IEnumerable<string> Filter(IEnumerable<string> words);
}