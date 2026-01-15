using System.Drawing;

namespace TagCloud.Abstractions;

public interface ICenterShifter
{
    Rectangle ShiftToCenter(Rectangle rectangle, Point center, IEnumerable<Rectangle> others);
}