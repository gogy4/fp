using System.Drawing;
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
    public void Generate(string outputPath, TagCloudVisualizationConfig config, int minFont = 10, int maxFont = 64,
        int maxTags = 500)
    {
        var raw = wordsSource.ReadWords();
        var processed = raw.Select(preprocessor.Preprocess).Where(filter.ShouldKeep);
        var freqs = freqCalc.Calculate(processed);

        if (!freqs.Any()) throw new InvalidOperationException("No words to render after preprocessing/filtering.");

        var top = freqs.OrderByDescending(kv => kv.Value).Take(maxTags).ToList();
        var minFreq = top.Min(kv => kv.Value);
        var maxFreq = top.Max(kv => kv.Value);

        var tagPlacer = tagPlacerFactory();

        var placed = new List<(Tag tag, Rectangle rect)>();
        foreach (var tag in from kv in top
                 let fontSize = sizeMapper.Map(kv.Value, minFont, maxFont, minFreq, maxFreq)
                 select new Tag(kv.Key, kv.Value, fontSize))
        {
            using var tmpBmp = new Bitmap(1, 1);
            using var g = Graphics.FromImage(tmpBmp);
            var font = new Font(config.FontName, tag.FontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var sizeF = g.MeasureString(tag.Text, font);
            var size = new Size((int)Math.Ceiling(sizeF.Width) + 6, (int)Math.Ceiling(sizeF.Height) + 6);

            var rect = tagPlacer.PutNextRectangle(size);
            placed.Add((tag, rect));
        }

        renderer.Render(placed, outputPath, config);
    }
}