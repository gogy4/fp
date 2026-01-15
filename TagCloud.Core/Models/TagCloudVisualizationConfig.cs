using System.Drawing;

namespace TagCloud.Models;

public class TagCloudVisualizationConfig
{
    public int CanvasWidth { get; set; }
    public int CanvasHeight { get; set; }
    public Color CanvasBackgroundColor { get; set; }
    public Color ShapeFillColor { get; set; }
    public Color ShapeBorderColor { get; set; }
    public int ShapeBorderThickness { get; set; }
    public string FontName { get; set; }
}