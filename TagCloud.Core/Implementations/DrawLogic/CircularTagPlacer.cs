using System.Drawing;
using ErrorHandling;
using TagCloud.Abstractions;

namespace TagCloud.Implementations;

public class CircularTagPlacer(CircularCloudLayouterBase layouter) : ITagPlacer
{
    public Result<Rectangle> PutNextRectangle(Size size)
    {
        return layouter.PutNextRectangle(size);
    }

    public IReadOnlyCollection<Rectangle> Rectangles => layouter.Rectangles;
}