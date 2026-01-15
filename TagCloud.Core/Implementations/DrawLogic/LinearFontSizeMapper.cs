using TagCloud.Abstractions;

namespace TagCloud.Implementations;

public class LinearFontSizeMapper : IFontSizeMapper
{
    public int Map(int frequency, int minFont, int maxFont, int minFreq, int maxFreq)
    {
        if (minFreq == maxFreq)
            return (minFont + maxFont) / 2;

        var lowFreq = Math.Min(minFreq, maxFreq);
        var highFreq = Math.Max(minFreq, maxFreq);
        var clampedFreq = Math.Min(Math.Max(frequency, lowFreq), highFreq);

        var frac = (double)(clampedFreq - minFreq) / (maxFreq - minFreq);

        var value = minFont + (int)Math.Round(frac * (maxFont - minFont), MidpointRounding.AwayFromZero);

        var lowFont = Math.Min(minFont, maxFont);
        var highFont = Math.Max(minFont, maxFont);
        return Math.Min(Math.Max(value, lowFont), highFont);
    }
}