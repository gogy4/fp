using FluentAssertions;
using TagCloud.Implementations;

namespace TagCloud.Tests;

[TestFixture]
public class LinearFontSizeMapperTests
{
    private readonly LinearFontSizeMapper mapper = new();

    [TestCaseSource(nameof(GetTestCases))]
    public void Map_ShouldReturnExpectedFontSize(int frequency, int minFont, int maxFont, int minFreq, int maxFreq,
        int expected)
    {
        var result = mapper.Map(frequency, minFont, maxFont, minFreq, maxFreq);

        result.Should().Be(expected,
            $"Expected correct linear interpolation for frequency={frequency} " +
            $"given freqRange=[{minFreq}, {maxFreq}] and fontRange=[{minFont}, {maxFont}].");
    }

    private static IEnumerable<TestCaseData> GetTestCases()
    {
        yield return new TestCaseData(5, 10, 50, 5, 5, 30)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenMinFreqEqualsMaxFreq")
            .SetDescription("If all frequencies are equal, mapper should return mid-font.");

        yield return new TestCaseData(1, 10, 50, 1, 10, 10)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenFrequencyEqualsMinFreq")
            .SetDescription("Frequency at minimum maps to minFont.");

        yield return new TestCaseData(10, 10, 50, 1, 10, 50)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenFrequencyEqualsMaxFreq")
            .SetDescription("Frequency at maximum maps to maxFont.");

        yield return new TestCaseData(2, 10, 50, 1, 3, 30)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenFrequencyIsInTheMiddleOfRange")
            .SetDescription("Frequency between min and max produces linear mid value.");

        yield return new TestCaseData(2, 10, 49, 1, 4, 23)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenRoundingIsRequired")
            .SetDescription("Mapper should round to nearest integer.");

        yield return new TestCaseData(0, 10, 50, 1, 10, 10)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenFrequencyBelowMinFreq")
            .SetDescription("Frequency lower than range should clamp to minFont.");

        yield return new TestCaseData(20, 10, 50, 1, 10, 50)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenFrequencyAboveMaxFreq")
            .SetDescription("Frequency above range should clamp to maxFont.");

        yield return new TestCaseData(-5, 10, 50, -10, 10, 20)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenFrequencyIsNegativeWithinRange")
            .SetDescription("Handles negative frequencies inside a negative range.");

        yield return new TestCaseData(-20, 10, 50, -10, 10, 10)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenFrequencyIsBelowNegativeRange")
            .SetDescription("Clamps below negative minFreq to minFont.");

        yield return new TestCaseData(20, 10, 50, -10, 10, 50)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenFrequencyExceedsNegativeRange")
            .SetDescription("Clamps above maxFreq to maxFont.");

        yield return new TestCaseData(5, 50, 10, 1, 10, 32)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenFontRangeIsReversed")
            .SetDescription("Supports inverted font ranges (maxFont < minFont).");

        yield return new TestCaseData(5, 10, 50, 10, 1, 32)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenFrequencyRangeIsReversed")
            .SetDescription("Works even when minFreq > maxFreq, inverse interpolation.");

        yield return new TestCaseData(500_000, 10, 100, 0, 1_000_000, 55)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenUsingLargeValues")
            .SetDescription("Handles large integers without overflow.");

        yield return new TestCaseData(1, 1, 3, 1, 4, 1)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenRoundingDown")
            .SetDescription("Linear interpolation should round down when fraction < .5.");

        yield return new TestCaseData(3, 1, 3, 1, 4, 2)
            .SetName("Map_ShouldReturnExpectedFontSize_WhenRoundingUp")
            .SetDescription("Linear interpolation should round up when fraction >= .5.");
    }
}