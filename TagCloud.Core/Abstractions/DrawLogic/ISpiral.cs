using System.Drawing;

namespace TagCloud.Abstractions;

public interface ISpiral
{
    Point GetNextPoint();
}