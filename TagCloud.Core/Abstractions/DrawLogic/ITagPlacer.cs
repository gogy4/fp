using System.Drawing;
using ErrorHandling;

namespace TagCloud.Abstractions;

public interface ITagPlacer
{
    Result<Rectangle> PutNextRectangle(Size size);
    IReadOnlyCollection<Rectangle> Rectangles { get; }
}