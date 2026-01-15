using FluentAssertions;
using TagCloud.Implementations;

namespace TagCloud.Tests;

[TestFixture]
public class TxtStopWordsProviderTests
{
    [SetUp]
    public void SetUp()
    {
        tempPath = Path.GetTempFileName();
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(tempPath))
            File.Delete(tempPath);
    }

    private string tempPath;

    [TestCaseSource(nameof(GetTestCases))]
    public void GetStopWords_ShouldReturnExpectedWords(string[] fileLines, IEnumerable<string> expected)
    {
        File.WriteAllLines(tempPath, fileLines);

        var provider = new TxtStopWordsProvider(tempPath);
        var result = provider.ReadWords();

        result.Should().BeEquivalentTo(expected,
            "TxtStopWordsProvider should lowercase words, trim whitespace, ignore empty lines, and return unique values.");
    }

    private static IEnumerable<TestCaseData> GetTestCases()
    {
        yield return new TestCaseData(new[] { "THE", "and", "", "And", "the  ", "  or" }, new[] { "the", "and", "or" })
            .SetName("GetStopWords_ShouldReturnLowercasedUniqueWords")
            .SetDescription("Ensures lowercasing and uniqueness for a mixed input of cases and duplicates.");

        yield return new TestCaseData(Array.Empty<string>(), Array.Empty<string>())
            .SetName("GetStopWords_ShouldReturnEmpty_WhenFileIsEmpty")
            .SetDescription("Empty file should produce no stop words.");

        yield return new TestCaseData(new[] { "", " ", "   " }, Array.Empty<string>())
            .SetName("GetStopWords_ShouldIgnoreOnlyWhitespaceLines")
            .SetDescription("Whitespace-only lines should be ignored.");

        yield return new TestCaseData(new[] { " A " }, new[] { "a" })
            .SetName("GetStopWords_ShouldTrimAndLowercaseSingleWord")
            .SetDescription("Single word should be trimmed and lowercased.");

        yield return new TestCaseData(new[] { "One", "TWO", "three" }, new[] { "one", "two", "three" })
            .SetName("GetStopWords_ShouldNormalizeCase_ForMultipleWords")
            .SetDescription("Words should be lowercased but kept distinct.");

        yield return new TestCaseData(new[] { "  spaced   ", "value" }, new[] { "spaced", "value" })
            .SetName("GetStopWords_ShouldTrimSpaces")
            .SetDescription("Leading and trailing spaces should be removed.");

        yield return new TestCaseData(new[] { "\t\thello\t" }, new[] { "hello" })
            .SetName("GetStopWords_ShouldTrimTabs")
            .SetDescription("Tabs should be trimmed like spaces.");

        yield return new TestCaseData(new[] { "HELLO ", " hello", " Hello " }, new[] { "hello" })
            .SetName("GetStopWords_ShouldDeduplicateAfterLowercasing")
            .SetDescription("Different casing versions of the same word should collapse into one.");

        yield return new TestCaseData(new[] { "hi!", "HI!", "hi?" }, new[] { "hi!", "hi?" })
            .SetName("GetStopWords_ShouldTreatPunctuationAsPartOfWord")
            .SetDescription("Punctuation should not be removed, and lowercasing should not affect punctuation.");

        yield return new TestCaseData(new[] { "русский", " ТЕКСТ ", "данные" }, new[] { "русский", "текст", "данные" })
            .SetName("GetStopWords_ShouldSupportUnicode")
            .SetDescription("Provider should handle Unicode words correctly.");

        yield return new TestCaseData(new[] { "123", " 456 ", "789" }, new[] { "123", "456", "789" })
            .SetName("GetStopWords_ShouldHandleNumericWords")
            .SetDescription("Numbers should be treated as valid stop words.");

        yield return new TestCaseData(new[] { "#", "!", "?" }, new[] { "#", "!", "?" })
            .SetName("GetStopWords_ShouldAllowSymbolOnlyWords")
            .SetDescription("Non-alphanumeric symbols should be accepted.");

        yield return new TestCaseData(new[] { "a", "", "b", " ", "c" }, new[] { "a", "b", "c" })
            .SetName("GetStopWords_ShouldIgnoreEmptyLinesBetweenWords")
            .SetDescription("Only non-empty trimmed lines count.");

        yield return new TestCaseData(new[] { "  A", "a  ", "A", "a" }, new[] { "a" })
            .SetName("GetStopWords_ShouldRemoveDuplicatesAfterTrimming")
            .SetDescription("Whitespace and casing variations should not produce duplicates.");

        yield return new TestCaseData(new[] { "\ufeffhello", "world" }, new[] { "hello", "world" })
            .SetName("GetStopWords_ShouldHandleUTF8BOM")
            .SetDescription("BOM-prefixed lines should be trimmed correctly.");

        yield return new TestCaseData(new[] { "two words", "  another sentence " },
                new[] { "two words", "another sentence" })
            .SetName("GetStopWords_ShouldNotSplitInternalSpaces")
            .SetDescription("The provider should not split multi-word lines; they are treated as whole tokens.");

        yield return new TestCaseData(new[] { "A!", "a!", "A?" }, new[] { "a!", "a?" })
            .SetName("GetStopWords_ShouldNormalizeCaseButKeepPunctuation")
            .SetDescription("Case-insensitive uniqueness but punctuation should remain unchanged.");

        yield return new TestCaseData(new[] { "test", "TEST ", " test" }, new[] { "test" })
            .SetName("GetStopWords_ShouldDeduplicateCompletely")
            .SetDescription("All variants of a word should collapse into a single stop word.");

        yield return new TestCaseData(Enumerable.Repeat("  LargeWord  ", 1000).ToArray(), new[] { "largeword" })
            .SetName("GetStopWords_ShouldHandleLargeFile")
            .SetDescription("Provider should work efficiently with large files.");

        yield return new TestCaseData(new[] { "a", "A", "aA", "Aa", "AA" }, new[] { "a", "aa" })
            .SetName("GetStopWords_ShouldNormalizeMixedCasePatterns")
            .SetDescription("Words with different case combinations should lower-case correctly.");
    }
}