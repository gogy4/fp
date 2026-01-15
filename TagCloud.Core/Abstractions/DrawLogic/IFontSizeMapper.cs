namespace TagCloud.Abstractions;

public interface IFontSizeMapper
{
    int Map(int frequency, int minFont, int maxFont, int minFreq, int maxFreq);
}