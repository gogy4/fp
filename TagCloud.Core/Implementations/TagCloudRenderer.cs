using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using TagCloud.Abstractions;
using TagCloud.Models;

namespace TagCloud.Implementations;

public class TagCloudRenderer : ITagCloudRenderer
{
    public void Render(IEnumerable<(Tag tag, Rectangle rect)> placedTags, string outputPath,
        TagCloudVisualizationConfig config)
    {
        using var bmp = new Bitmap(config.CanvasWidth, config.CanvasHeight);
        using var g = Graphics.FromImage(bmp);
        g.Clear(config.CanvasBackgroundColor);
        g.TextRenderingHint = TextRenderingHint.AntiAlias;

        foreach (var (tag, rect) in placedTags)
        {
            var font = new Font(config.FontName, tag.FontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            var drawRect = new Rectangle(rect.X + config.CanvasWidth / 2, rect.Y + config.CanvasHeight / 2, rect.Width,
                rect.Height);

            using var fillBrush = new SolidBrush(config.ShapeFillColor);
            g.FillRectangle(fillBrush, drawRect);

            using var pen = new Pen(config.ShapeBorderColor, config.ShapeBorderThickness);
            g.DrawRectangle(pen, drawRect);

            var text = tag.Text;
            g.DrawString(text, font, Brushes.White, drawRect, sf);
            font.Dispose();
        }

        bmp.Save(outputPath, ImageFormat.Png);
    }
}