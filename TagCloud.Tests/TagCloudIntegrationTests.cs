using System.Drawing;
using Autofac;
using FluentAssertions;
using TagCloud;
using TagCloud.DI;
using TagCloud.Models;

namespace TagCloud.Tests;

[TestFixture]
public class TagCloudIntegrationTests
{
    private string baseDir;
    private string outputPath;
    private TagCloudVisualizationConfig cfg;

    [SetUp]
    public void SetUp()
    {
        baseDir = Path.GetFullPath(Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "..", "..", ".."
        ));

        var testName = TestContext.CurrentContext.Test.Name;
        outputPath = Path.Combine(baseDir, $"cloud_{testName}.png");

        cfg = new TagCloudVisualizationConfig
        {
            CanvasWidth = 800,
            CanvasHeight = 800,
            ShapeFillColor = Color.Blue,
            ShapeBorderColor = Color.White
        };

        if (File.Exists(outputPath))
            File.Delete(outputPath);
    }

    private TagCloudGenerator Resolve(string words, string stopwords)
    {
        var testName = TestContext.CurrentContext.Test.Name;

        var wordsFile = Path.Combine(baseDir, $"words_{testName}.txt");
        var stopwordsFile = Path.Combine(baseDir, $"stopwords_{testName}.txt");

        File.WriteAllLines(wordsFile, (words ?? "").Split('\n'));
        File.WriteAllLines(stopwordsFile, (stopwords ?? "").Split('\n'));

        var builder = new ContainerBuilder();
        builder.RegisterModule(new TagCloudModule(wordsFile, stopwordsFile));

        var container = builder.Build();
        return container.Resolve<TagCloudGenerator>();
    }
    
    [Test]
    public void Generate_ShouldReadWordsAndStopwordsFromFiles()
    {
        var words = "apple\nbanana\nbanana\ncloud";
        var stopwords = "apple";

        var gen = Resolve(words, stopwords);

        var result = gen.Generate(outputPath, cfg);

        result.IsSuccess.Should().BeTrue("чтение слов и генерация должны пройти успешно");

        File.Exists(outputPath).Should().BeTrue("файл изображения должен быть создан");
        new FileInfo(outputPath).Length.Should().BeGreaterThan(50, "изображение не должно быть пустым");
    }
    
    [TestCaseSource(nameof(GetGenerationCases))]
    public void Generate_ShouldBehaveAsExpected(string words, string stop, bool shouldSucceed)
    {
        var gen = Resolve(words, stop);

        var result = gen.Generate(outputPath, cfg);

        if (shouldSucceed)
        {
            result.IsSuccess.Should().BeTrue("ожидалась успешная генерация");

            File.Exists(outputPath).Should().BeTrue("файл должен быть создан");
            new FileInfo(outputPath).Length.Should().BeGreaterThan(50, "картинка не должна быть пустой");
        }
        else
        {
            result.IsSuccess.Should().BeFalse("ожидалась ошибка генерации");
            result.Error.Should().NotBeNullOrWhiteSpace();
        }
    }

    [TestCaseSource(nameof(GetInvalidPathCases))]
    public void Generate_ShouldFail_OnInvalidOutputPath(string badPath)
    {
        var gen = Resolve("word\nword\ncloud", "");

        var result = gen.Generate(badPath, cfg);

        result.IsSuccess.Should().BeFalse("некорректный путь должен приводить к ошибке");
        result.Error.Should().NotBeNullOrWhiteSpace();
    }

    public static IEnumerable<TestCaseData> GetInvalidPathCases()
    {
        yield return new TestCaseData(null)
            .SetName("Generate_ShouldFail_WhenOutputPathIsNull");

        yield return new TestCaseData("   ")
            .SetName("Generate_ShouldFail_WhenOutputPathIsWhitespace");

        yield return new TestCaseData("invalid/file\\path.png")
            .SetName("Generate_ShouldFail_WhenOutputPathInvalid");
    }

    public static IEnumerable<TestCaseData> GetGenerationCases()
    {
        yield return new TestCaseData(
                "apple\napple\nbanana\nbanana\nbanana\ncloud\ncloud\ncloud",
                "",
                true)
            .SetName("Generate_ShouldSucceed_WhenStopwordsEmpty");

        yield return new TestCaseData(
                "apple\nbanana\norange",
                "banana",
                true)
            .SetName("Generate_ShouldFilterStopwords");

        yield return new TestCaseData(
                "one\nTWO\nthree\nTwo",
                "two",
                true)
            .SetName("Generate_ShouldBeCaseInsensitiveForStopwords");

        yield return new TestCaseData(
                "",
                "",
                false)
            .SetName("Generate_ShouldFail_WhenNoWordsProvided");

        yield return new TestCaseData(
                "a\nb\nc",
                "a\nb\nc",
                false)
            .SetName("Generate_ShouldFail_WhenAllWordsFilteredOut");
    }
}
