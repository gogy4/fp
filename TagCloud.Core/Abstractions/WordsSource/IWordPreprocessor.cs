namespace TagCloud.Abstractions;

public interface IWordPreprocessor
{
    string Preprocess(string word);
    IEnumerable<string> PreprocessMany(IEnumerable<string> words);
}