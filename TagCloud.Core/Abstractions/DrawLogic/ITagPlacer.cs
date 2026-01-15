using System.Drawing;

namespace TagCloud.Abstractions;

public interface ITagPlacer
{
    IReadOnlyCollection<Rectangle> Rectangles { get; }
    Rectangle PutNextRectangle(Size size);
}