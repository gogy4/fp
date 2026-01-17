using FluentAssertions;
using TagCloud.Implementations;
using TagCloud.Implementations.WordSource;

namespace TagCloud.Tests;

[TestFixture]
public class TxtWordsSourceTests
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
    public void ReadWords_ShouldReturnExpectedWords(string[] fileLines, IEnumerable<string> expected)
    {
        File.WriteAllLines(tempPath, fileLines);

        var source = new TxtWordsSource(tempPath);
        var result = source.ReadWords().ToArray();

        result.Should().BeEquivalentTo(expected,
            "TxtWordsSource should trim lines and ignore empty results.");
    }

    private static IEnumerable<TestCaseData> GetTestCases()
    {
        yield return new TestCaseData(new[] { "hello", "  world  ", "", "   ", "test" },
                new[] { "hello", "world", "test" })
            .SetName("ReadWords_ShouldReturnTrimmedNonEmptyLines")
            .SetDescription("Trims whitespace on each line and skips empty/whitespace-only lines.");

        yield return new TestCaseData(Array.Empty<string>(), Array.Empty<string>())
            .SetName("ReadWords_ShouldReturnEmpty_WhenFileIsEmpty")
            .SetDescription("Empty file → empty result.");

        yield return new TestCaseData(new[] { "", " ", "   " }, Array.Empty<string>())
            .SetName("ReadWords_ShouldIgnoreOnlyWhitespaceLines")
            .SetDescription("Whitespace-only lines are ignored after trimming.");

        yield return new TestCaseData(new[] { "  a  " }, new[] { "a" })
            .SetName("ReadWords_ShouldTrimSingleLine")
            .SetDescription("Single trimmed line should return the inner word.");

        yield return new TestCaseData(new[] { "Line1", "Line2", "Line3" }, new[] { "line1", "line2", "line3" })
            .SetName("ReadWords_ShouldReadMultipleSimpleLines")
            .SetDescription("Multiple lines without extra spaces are returned unchanged.");

        yield return new TestCaseData(new[] { "   spaced   ", "value" }, new[] { "spaced", "value" })
            .SetName("ReadWords_ShouldTrimLeadingAndTrailingSpaces")
            .SetDescription("Leading and trailing spaces are removed from all lines.");

        yield return new TestCaseData(new[] { "\t\thello\t" }, new[] { "hello" })
            .SetName("ReadWords_ShouldTrimTabs")
            .SetDescription("Tabs are treated as whitespace and trimmed.");

        yield return new TestCaseData(new[] { "\nhello", "world\n" }, new[] { "hello", "world" })
            .SetName("ReadWords_ShouldTrimNewlineCharacters")
            .SetDescription("Newline characters at the edges are trimmed.");

        yield return new TestCaseData(new[] { "hello ", " hello", " hello " }, new[] { "hello", "hello", "hello" })
            .SetName("ReadWords_ShouldTreatTrimmedWordsAsEqual_ButReturnAll")
            .SetDescription("After trimming, identical words remain duplicates and must all be returned.");

        yield return new TestCaseData(new[] { "HELLO", "hello", "Hello" }, new[] { "hello", "hello", "hello" })
            .SetName("ReadWords_ShouldPreserveCase")
            .SetDescription("Trimming does not affect letter casing.");

        yield return new TestCaseData(new[] { "word!", "!word", "wo!rd" }, new[] { "word!", "!word", "wo!rd" })
            .SetName("ReadWords_ShouldNotRemovePunctuation")
            .SetDescription("Punctuation inside and around words is preserved.");

        yield return new TestCaseData(new[] { "    many    spaces    " }, new[] { "many    spaces" })
            .SetName("ReadWords_ShouldNotCollapseInternalSpaces")
            .SetDescription("Internal spacing is preserved; only outer spaces are trimmed.");

        yield return new TestCaseData(new[] { "123", " 456 ", "789" }, new[] { "123", "456", "789" })
            .SetName("ReadWords_ShouldHandleNumericLines")
            .SetDescription("Numbers are treated as words and trimmed normally.");

        yield return new TestCaseData(new[] { "русский", " текст ", "данные" }, new[] { "русский", "текст", "данные" })
            .SetName("ReadWords_ShouldHandleUnicodeWords")
            .SetDescription("Unicode text is handled correctly and trimmed.");

        yield return new TestCaseData(new[] { "\t   mixed \t spaces\t" }, new[] { "mixed \t spaces" })
            .SetName("ReadWords_ShouldPreserveInternalTabs")
            .SetDescription("Tabs and spaces inside the word remain unchanged.");

        yield return new TestCaseData(new[] { "a", "", "b", " ", "c" }, new[] { "a", "b", "c" })
            .SetName("ReadWords_ShouldSkipEmptyLinesInBetween")
            .SetDescription("Empty and whitespace-only lines between valid lines are ignored.");

        yield return new TestCaseData(new[] { " a ", " a ", "  a", "a  " }, new[] { "a", "a", "a", "a" })
            .SetName("ReadWords_ShouldReturnDuplicatesAfterTrim")
            .SetDescription("Trimmed duplicates must be returned as individual items.");

        yield return new TestCaseData(new[] { "a\n", "\tb" }, new[] { "a", "b" })
            .SetName("ReadWords_ShouldHandleWeirdTrimCombinations")
            .SetDescription("Combination of newlines and tabs is trimmed correctly.");

        yield return new TestCaseData(new[] { "line with spaces", " another   line " },
                new[] { "line with spaces", "another   line" })
            .SetName("ReadWords_ShouldHandleSentences")
            .SetDescription("Multi-word sentences keep internal spacing and get trimmed externally.");

        yield return new TestCaseData(Enumerable.Repeat("   skip   ", 1000).ToArray(),
                Enumerable.Repeat("skip", 1000))
            .SetName("ReadWords_ShouldHandleLargeFile")
            .SetDescription("Large amount of lines processed correctly with trimming.");

        yield return new TestCaseData(
                new[] { ":", " , ", " ! " },
                new[] { ":", ",", "!" })
            .SetName("PunctuationIsWord")
            .SetDescription("Standalone punctuation is considered a valid word after trimming.");

        yield return new TestCaseData(new[] { " ", "   ", "\t" }, Array.Empty<string>())
            .SetName("ReadWords_ShouldIgnoreWhitespaceOnlyLines")
            .SetDescription("Whitespace-only lines are ignored, even if tabs.");

        yield return new TestCaseData(new[] { "a", "\0", "b" }, new[] { "a", "\0", "b" })
            .SetName("ReadWords_ShouldAllowNullCharInsideWord")
            .SetDescription("Null character inside a line is preserved after trimming.");

        yield return new TestCaseData(new[] { "########" }, new[] { "########" })
            .SetName("ReadWords_ShouldAllowSpecialSymbolOnlyWords")
            .SetDescription("Lines consisting only of symbols remain unchanged after trimming.");

        yield return new TestCaseData(
                new[] { "ascii", "ümlaut", "中文" },
                new[] { "ascii", "ümlaut", "中文" })
            .SetName("ReadWords_ShouldHandleMixedLanguageLines")
            .SetDescription("Words from different languages are handled and trimmed properly.");
    }
}