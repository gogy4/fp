using System.Drawing;
using Autofac;
using FluentAssertions;
using TagCloud.DI;
using TagCloud.Models;

namespace TagCloud.Tests;

[TestFixture]
public class TagCloudIntegrationTests
{
    [SetUp]
    public void SetUp()
    {
        baseDir = Path.GetFullPath(Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "..", "..", ".."
        ));
        wordsPath = Path.Combine(baseDir, "testwords.txt");
        stopwordsPath = Path.Combine(baseDir, "teststopwords.txt");

        var testName = TestContext.CurrentContext.Test.Name;
        outputPath = Path.Combine(baseDir, $"cloud_{testName}.png");
        cfg = new TagCloudVisualizationConfig
        {
            CanvasWidth = 800,
            CanvasHeight = 800,
            ShapeFillColor = Color.Blue,
            ShapeBorderColor = Color.White
        };

        if (File.Exists(outputPath)) File.Delete(outputPath);

        File.WriteAllText(wordsPath, "");
        File.WriteAllText(stopwordsPath, "");
    }

    private string wordsPath;
    private string stopwordsPath;
    private string outputPath;
    private TagCloudVisualizationConfig cfg;
    private string baseDir;


    private TagCloudGenerator Resolve(string words = null, string stopwords = null, bool writeFile = true)
    {
        var testName = TestContext.CurrentContext.Test.Name;

        // уникальные файлы для каждого теста
        var wordsFile = Path.Combine(baseDir, $"words_{testName}.txt");
        var stopwordsFile = Path.Combine(baseDir, $"stopwords_{testName}.txt");

        if (writeFile)
        {
            File.WriteAllLines(wordsFile, (words ?? "").Split('\n'));
            File.WriteAllLines(stopwordsFile, (stopwords ?? "").Split('\n'));
        }

        var builder = new ContainerBuilder();
        builder.RegisterModule(new TagCloudModule(wordsFile, stopwordsFile));

        var container = builder.Build();
        return container.Resolve<TagCloudGenerator>();
    }


    [Test]
    public void Generate_ShouldReadWordsAndStopwordsFromFiles()
    {
        var gen = Resolve(wordsPath, stopwordsPath, false);
        var act = () => gen.Generate(outputPath, cfg);
        act.Should().NotThrow("reading from test files should work");
        File.Exists(outputPath).Should().BeTrue("output image must be created");
        new FileInfo(outputPath).Length.Should().BeGreaterThan(50, "image must not be empty");
    }


    [TestCaseSource(nameof(GetGenerationCases))]
    public void Generate_ShouldBehaveAsExpected(string words, string stop, bool shouldSucceed)
    {
        var gen = Resolve(words, stop);

        var act = () => gen.Generate(outputPath, cfg);

        if (shouldSucceed)
        {
            act.Should().NotThrow("должно успешно создать изображение");
            File.Exists(outputPath).Should().BeTrue("файл должен быть создан");
            new FileInfo(outputPath).Length.Should().BeGreaterThan(50, "картинка не должна быть пустой");
        }
        else
        {
            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("No words to render after preprocessing/filtering.");
        }
    }

    [TestCaseSource(nameof(GetErrorCases))]
    public void Generate_ShouldThrow_OnInvalidOutputPath(string words, string stop, string badPath)
    {
        var gen = Resolve(words, stop);

        var act = () => gen.Generate(badPath, cfg);

        act.Should()
            .Throw<Exception>("генерация невозможна при неверном пути файла");
    }

    public static IEnumerable<TestCaseData> GetErrorCases()
    {
        yield return new TestCaseData(
                "word",
                "",
                null)
            .SetName("Generate_ShouldThrow_WhenOutputPathIsNull")
            .SetDescription("outputPath = null → should throw an exception");

        yield return new TestCaseData(
                "word",
                "",
                "   ")
            .SetName("Generate_ShouldThrow_WhenOutputPathIsWhitespace")
            .SetDescription("output path containing only whitespace → should throw an exception");

        yield return new TestCaseData(
                "word",
                "",
                "invalid/file\\path.png")
            .SetName("Generate_ShouldThrow_WhenOutputPathInvalid")
            .SetDescription("invalid output file path → should throw an exception");
    }

    public static IEnumerable<TestCaseData> GetGenerationCases()
    {
        yield return new TestCaseData(
                "apple\napple\nbanana\nbanana\nbanana\nsky\nsky\nsky\ncloud\ncloud\n" +
                "cloud\ncloud\nhello\nhello\nhello\nworld\nworld\nworld\nworld\nworld\n" +
                "rain\nrain\nrain\nrain\nrain\nrain\nflower\nflower\nflower\nflower\nf" +
                "lower\nsun\nsun\nsun\nsun\nsun\nmoon\nmoon\nmoon\nmoon\nmoon\nmoon\n" +
                "star\nstar\nstar\nstar\nstar\nstar\nstar\ntree\ntree\ntree\ntree\ntr" +
                "ee\ntree\ntree\ntree\nriver\nriver\nriver\nriver\nriver\nriver\nmoun" +
                "tain\nmountain\nmountain\nmountain\nmountain\nmountain\nmountain\nmou" +
                "ntain\nmountain\nwind\nwind\nwind\nwind\nwind\nwind\nwind\nwind\nwin" +
                "d\nwind\nstone\nstone\nstone\nstone\nstone\nstone\nstone\nstone\nston" +
                "e\nfire\nfire\nfire\nfire\nfire\nfire\nfire\nfire\nfire\nfire\ncloudy" +
                "\ncloudy\ncloudy\ncloudy\ncloudy\ncloudy\ncloudy\ncloudy\ncloudy\nclou" +
                "dy\nrainbow\nrainbow\nrainbow\nrainbow\nrainbow\nrainbow\nrainbow\nrainbo" +
                "w\nrainbow\nrainbow\nleaf\nleaf\nleaf\nleaf\nleaf\nleaf\nleaf\nleaf\nleaf\nleaf",
                "hello\nthe\nand\na\nof\nin\non\nat\nfor\nrain\nsun",
                true)
            .SetName("Generate_ShouldProduceImage_WhenStopwordsEmpty")
            .SetDescription("Verifies successful image generation when no stopwords are provided.");

        yield return new TestCaseData(
                "apple\nbanana\norange",
                "banana",
                true)
            .SetName("Generate_ShouldFilterStopwordsAndProduceImage")
            .SetDescription("Stopwords should be filtered out and the image should still be generated.");

        yield return new TestCaseData(
                "one\nTWO\nthree\nTwo",
                "two",
                true)
            .SetName("Generate_ShouldBeCaseInsensitiveForStopwords")
            .SetDescription("Stopword 'two' should filter out 'TWO' and 'Two' (case-insensitive).");

        yield return new TestCaseData(
                "",
                "",
                false)
            .SetName("Generate_ShouldFail_WhenNoWordsProvided")
            .SetDescription("An error is expected when the words file is empty.");

        yield return new TestCaseData(
                "a\nb\nc",
                "a\nb\nc",
                false)
            .SetName("Generate_ShouldFail_WhenAllWordsFilteredOut")
            .SetDescription("All words are filtered out, so the generator should throw an error.");
    }
}