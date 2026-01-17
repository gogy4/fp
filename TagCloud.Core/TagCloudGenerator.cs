using System.Drawing;
using ErrorHandling;
using Autofac.Features.AttributeFilters;
using TagCloud.Abstractions;
using TagCloud.Abstractions.WordsSource;
using TagCloud.Models;

namespace TagCloud;

public class TagCloudGenerator(
    [KeyFilter("words")] IWordsSource wordsSource,
    IWordPreprocessor preprocessor,
    IWordFilter filter,
    IFrequencyCalculator freqCalc,
    IFontSizeMapper sizeMapper,
    Func<ITagPlacer> tagPlacerFactory,
    ITagCloudRenderer renderer)
{
    public Result<None> Generate(string outputPath, TagCloudVisualizationConfig config, int minFont = 10, int maxFont = 64,
        int maxTags = 500)
    {
        return Result.Of(() => wordsSource.ReadWords(), "Ошибка при чтении слов")
            .Then(raw => Result.Of(
                () => raw.Select(preprocessor.Preprocess).Where(filter.ShouldKeep).ToList(),
                "Ошибка при обработке или фильтрации слов"))
            .Then(processed =>
            {
                var freqs = Result.Of(() => freqCalc.Calculate(processed), "Ошибка при подсчёте частот");

                return freqs.Then(f =>
                {
                    if (!f.Any())
                        return Result.Fail<None>("Нет слов для визуализации после фильтрации и обработки.");

                    var top = f.OrderByDescending(kv => kv.Value).Take(maxTags).ToList();
                    var minFreq = top.Min(kv => kv.Value);
                    var maxFreq = top.Max(kv => kv.Value);

                    var tagPlacer = tagPlacerFactory();
                    var placed = new List<(Tag tag, Rectangle rect)>();

                    foreach (var kv in top)
                    {
                        var tag = Result.Of(() =>
                        {
                            var fontSize = sizeMapper.Map(kv.Value, minFont, maxFont, minFreq, maxFreq);
                            return new Tag(kv.Key, kv.Value, fontSize);
                        }, $"Ошибка при создании тега для слова {kv.Key}")
                        .GetValueOrThrow();

                        var rectResult = Result.Of(() =>
                        {
                            using var tmpBmp = new Bitmap(1, 1);
                            using var g = Graphics.FromImage(tmpBmp);
                            var font = new Font(config.FontName, tag.FontSize, FontStyle.Bold, GraphicsUnit.Pixel);
                            var sizeF = g.MeasureString(tag.Text, font);
                            return new Size((int)Math.Ceiling(sizeF.Width) + 6, (int)Math.Ceiling(sizeF.Height) + 6);
                        }, $"Ошибка при измерении размера тега {tag.Text}")
                        .Then(size => tagPlacer.PutNextRectangle(size));

                        if (!rectResult.IsSuccess)
                            return Result.Fail<None>(rectResult.Error);
                        
                        var rect = rectResult.GetValueOrThrow();
                        placed.Add((tag, rect));
                    }

                    return Result.Of(() => 
                    {
                        renderer.Render(placed, outputPath, config);
                        return None.Value;
                    }, "Ошибка при рендеринге облака тегов");
                });
            });
    }
}
