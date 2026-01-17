using System.Drawing;
using Autofac;
using ErrorHandling;
using TagCloud.Models;


namespace TagCloud.Client;

public static class TagCloudGeneratorHelper
{
    public static Result<None> GenerateTagCloud((IContainer container, ProgramArgs args) input)
    {
        return Result.Of(() =>
        {
            var (container, args) = input;

            var config = new TagCloudVisualizationConfig
            {
                CanvasWidth = args.Width,
                CanvasHeight = args.Height,
                CanvasBackgroundColor = Color.Black,
                ShapeFillColor = Color.DarkSlateGray,
                ShapeBorderColor = Color.White,
                ShapeBorderThickness = 1,
                FontName = args.FontName
            };

            using var scope = container.BeginLifetimeScope();
            var generator = scope.Resolve<TagCloudGenerator>();

            generator.Generate(args.OutputFile, config);

            Console.WriteLine($"Файл сохранён: {args.OutputFile}");
            return None.Value;
        }, "Ошибка при генерации облака тегов");
    }
}