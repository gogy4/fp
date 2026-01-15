using System.Drawing;
using TagCloud.Models;

namespace TagCloud.Abstractions;

public interface ITagCloudRenderer
{
    void Render(IEnumerable<(Tag tag, Rectangle rect)> placedTags, string outputPath,
        TagCloudVisualizationConfig config);
}