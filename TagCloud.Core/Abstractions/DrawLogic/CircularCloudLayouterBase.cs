using System.Drawing;

namespace TagCloud.Abstractions;

public abstract class CircularCloudLayouterBase(Point center)
{
    private readonly List<Rectangle> rectangles = [];
    public Point Center { get; } = center;
    public IReadOnlyList<Rectangle> Rectangles => rectangles;

    protected void AddRectangle(Rectangle rectangle)
    {
        rectangles.Add(rectangle);
    }

    public abstract Rectangle PutNextRectangle(Size rectangleSize);
}