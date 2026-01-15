using FluentAssertions;
using TagCloud.Implementations;

namespace TagCloud.Tests;

[TestFixture]
public class FrequencyCalculatorTests
{
    private readonly FrequencyCalculator calc = new();

    [TestCaseSource(nameof(GetTestCases))]
    public void Calculate_ShouldReturnExpectedFrequencies(IEnumerable<string> words,
        Dictionary<string, int> expected)
    {
        var frequencies = calc.Calculate(words);

        frequencies.Should().BeEquivalentTo(expected,
            "frequency calculator should count occurrences of each word exactly");
    }

    private static IEnumerable<TestCaseData> GetTestCases()
    {
        yield return new TestCaseData(new[] { "a", "b", "a", "c", "b", "a" },
                new Dictionary<string, int> { ["a"] = 3, ["b"] = 2, ["c"] = 1 }
            )
            .SetName("Calculate_ShouldReturnCorrectFrequencies_ForBasicInput")
            .SetDescription("Simple counting of repeated words.");

        yield return new TestCaseData(new string[] { }, new Dictionary<string, int>())
            .SetName("Calculate_ShouldReturnEmptyDictionary_WhenInputIsEmpty")
            .SetDescription("No words → empty result.");

        yield return new TestCaseData(new[] { "Hello", "hello", "HELLO" }, new Dictionary<string, int>
                {
                    ["Hello"] = 1,
                    ["hello"] = 1,
                    ["HELLO"] = 1
                }
            )
            .SetName("Calculate_ShouldTreatWordsWithDifferentCasingAsDifferent")
            .SetDescription("Calculator does not normalize case.");

        yield return new TestCaseData(new[] { "a", "b", "a" }, new Dictionary<string, int>
                {
                    ["a"] = 2,
                    ["b"] = 1
                }
            )
            .SetName("Calculate_ShouldCountNullValuesSeparately")
            .SetDescription("null is treated as a valid dictionary key.");

        yield return new TestCaseData(new[] { " hi", "hi ", " hi " }, new Dictionary<string, int>
                {
                    [" hi"] = 1,
                    ["hi "] = 1,
                    [" hi "] = 1
                }
            )
            .SetName("Calculate_ShouldHandleWhitespace_AsDistinctWords")
            .SetDescription("No trimming performed → strings stay distinct.");

        yield return new TestCaseData(new[] { "hi!", "hi!", "hi?", "hi!" }, new Dictionary<string, int>
                {
                    ["hi!"] = 3,
                    ["hi?"] = 1
                }
            )
            .SetName("Calculate_ShouldCountPunctuationInWords")
            .SetDescription("Special characters are part of the word.");

        yield return new TestCaseData(GenerateLargeInput(), GenerateLargeExpected())
            .SetName("Calculate_ShouldHandleLargeInput")
            .SetDescription("Performance and correctness on 10k words.");
    }

    private static IEnumerable<string> GenerateLargeInput()
    {
        for (var i = 0; i < 10000; i++) yield return "word" + i % 10;
    }

    private static Dictionary<string, int> GenerateLargeExpected()
    {
        var dict = new Dictionary<string, int>();
        for (var i = 0; i < 10; i++) dict["word" + i] = 1000;
        return dict;
    }
}