using System.Drawing;
using ErrorHandling;

namespace TagCloud.Abstractions;

public abstract class CircularCloudLayouterBase(Point center)
{
    private readonly List<Rectangle> rectangles = [];

    public Point Center { get; } = center;
    public IReadOnlyList<Rectangle> Rectangles => rectangles;

    protected Result<None> AddRectangle(Rectangle rectangle)
    {
        rectangles.Add(rectangle);
        return Result.Ok(None.Value);
    }

    public abstract Result<Rectangle> PutNextRectangle(Size rectangleSize);
}