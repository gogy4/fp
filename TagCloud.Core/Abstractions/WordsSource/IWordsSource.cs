namespace TagCloud.Abstractions.WordsSource;

public interface IWordsSource
{
    public IEnumerable<string> ReadWords();
}