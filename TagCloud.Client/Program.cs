using System.Drawing;
using Autofac;
using TagCloud;
using TagCloud.DI;
using TagCloud.Models;

public class Program
{
    public static int Main(string[] args)
    {
        try
        {
            if (args.Length < 3)
            {
                Console.WriteLine("не переданы аргументы");
                return 1;
            }

            var wordsFile = args[0];
            var stopWordsFile = args[1];
            var outputFile = args[2];

            var width = args.Length > 3 ? int.Parse(args[3]) : 1200;
            var height = args.Length > 4 ? int.Parse(args[4]) : 800;
            var fontName = args.Length > 5 ? args[5] : "Arial";

            var builder = new ContainerBuilder();
            builder.RegisterModule(new TagCloudModule(wordsFile, stopWordsFile));

            var container = builder.Build();

            var config = new TagCloudVisualizationConfig
            {
                CanvasWidth = width,
                CanvasHeight = height,
                CanvasBackgroundColor = Color.Black,
                ShapeFillColor = Color.DarkSlateGray,
                ShapeBorderColor = Color.White,
                ShapeBorderThickness = 1,
                FontName = fontName
            };


            using var scope = container.BeginLifetimeScope();
            var generator = scope.Resolve<TagCloudGenerator>();
            generator.Generate(outputFile, config);

            Console.WriteLine($"Файл сохранён: {outputFile}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при выполнении: " + ex.Message);
            return -1;
        }
    }
}