using FluentAssertions;
using TagCloud.Implementations;

namespace TagCloud.Tests;

[TestFixture]
public class LowercasePreprocessorTests
{
    private readonly LowercasePreprocessor pre = new();

    [TestCaseSource(nameof(GetTestCases))]
    public void PreprocessMany_ShouldReturnExpectedWords(IEnumerable<string> words, IEnumerable<string> expected)
    {
        var result = pre.PreprocessMany(words).ToArray();
        result.Should().BeEquivalentTo(expected,
            "LowercasePreprocessor should convert all words to lowercase without modifying anything else.");
    }

    private static IEnumerable<TestCaseData> GetTestCases()
    {
        yield return new TestCaseData(new[] { "Hello", "WORLD", "TeSt" }, new[] { "hello", "world", "test" })
            .SetName("PreprocessMany_ShouldLowercaseBasicWords")
            .SetDescription("Ensures typical words are converted to lowercase.");

        yield return new TestCaseData(Array.Empty<string>(), Array.Empty<string>())
            .SetName("PreprocessMany_ShouldReturnEmpty_WhenInputIsEmpty")
            .SetDescription("Empty input should produce empty output.");

        yield return new TestCaseData(new[] { "hello", "world" }, new[] { "hello", "world" })
            .SetName("PreprocessMany_ShouldNotModifyAlreadyLowercaseWords")
            .SetDescription("Words already in lowercase should remain unchanged.");

        yield return new TestCaseData(new[] { "HELLO", "WORLD" }, new[] { "hello", "world" })
            .SetName("PreprocessMany_ShouldHandleAllUppercaseWords")
            .SetDescription("Words in all-uppercase are converted properly.");

        yield return new TestCaseData(new[] { "hElLo", "WoRlD" }, new[] { "hello", "world" })
            .SetName("PreprocessMany_ShouldHandleMixedCaseWords")
            .SetDescription("Mixed-case words should be fully lowercased.");

        yield return new TestCaseData(new[] { "русский", "ТЕКСТ", "сЛоВо" }, new[] { "русский", "текст", "слово" })
            .SetName("PreprocessMany_ShouldHandleUnicodeLetters")
            .SetDescription("Unicode characters should be lowercased using invariant rules.");

        yield return new TestCaseData(new[] { "123", "456" }, new[] { "123", "456" })
            .SetName("PreprocessMany_ShouldIgnoreNumbers")
            .SetDescription("Numbers remain unchanged because lowercase does not affect digits.");

        yield return new TestCaseData(
                new[] { "HELLO!", "TeSt?", "!WORLD" },
                new[] { "hello!", "test?", "!world" })
            .SetName("PreprocessMany_ShouldPreservePunctuation")
            .SetDescription("Punctuation should remain unchanged while letters are lowercased.");

        yield return new TestCaseData(new[] { "   TEST   ", "  WORD " }, new[] { "   test   ", "  word " })
            .SetName("PreprocessMany_ShouldNotTrimWhitespace")
            .SetDescription("Whitespace is not removed or modified—only letters are lowercased.");

        yield return new TestCaseData(new[] { "\tTEST\t", "WOW\n" }, new[] { "\ttest\t", "wow\n" })
            .SetName("PreprocessMany_ShouldPreserveWhitespaceCharacters")
            .SetDescription("Tabs and newlines remain intact.");

        yield return new TestCaseData(new[] { "A", "a", "A", "a" }, new[] { "a", "a", "a", "a" })
            .SetName("PreprocessMany_ShouldProcessDuplicates")
            .SetDescription("Duplicates should be transformed independently.");

        yield return new TestCaseData(new[] { "hello world", "TEST STRING" }, new[] { "hello world", "test string" })
            .SetName("PreprocessMany_ShouldHandleWordsWithSpacesInside")
            .SetDescription("Lowercasing should apply to each character but spacing remains unchanged.");

        yield return new TestCaseData(new[] { "WORD1", "WORD2", "WORD3" }, new[] { "word1", "word2", "word3" })
            .SetName("PreprocessMany_ShouldLowercaseAlphanumericWords")
            .SetDescription("Alphanumeric words should lowercase their alphabetic part only.");

        yield return new TestCaseData(Enumerable.Repeat("TESTWORD", 1000), Enumerable.Repeat("testword", 1000))
            .SetName("PreprocessMany_ShouldHandleLargeInput")
            .SetDescription("Large input should be processed correctly and efficiently.");
    }
}