using System.Drawing;
using System.Drawing.Text;
using Autofac;
using ErrorHandling;
using TagCloud;
using TagCloud.DI;
using TagCloud.Models;

namespace TagCloud.Client;

public static class ProgramArgsValidator
{
    public static Result<string[]> ValidateArgs(string[] args)
    {
        return args.Length < 3
            ? Result.Fail<string[]>("Не переданы аргументы")
            : Result.Ok(args);
    }

    public static Result<ProgramArgs> ParseArgs(string[] args)
    {
        return Result.Of(() =>
        {
            var width = args.Length > 3 ? int.Parse(args[3]) : 1200;
            var height = args.Length > 4 ? int.Parse(args[4]) : 800;
            var fontName = args.Length > 5 ? args[5] : "Arial";

            return new ProgramArgs(
                args[0],
                args[1],
                args[2],
                width,
                height,
                fontName
            );
        }, "Ошибка при разборе аргументов");
    }

    public static Result<ProgramArgs> ValidateFiles(ProgramArgs args)
    {
        if (!File.Exists(args.WordsFile))
            return Result.Fail<ProgramArgs>($"Файл слов не найден: {args.WordsFile}");

        return !File.Exists(args.StopWordsFile)
            ? Result.Fail<ProgramArgs>($"Файл стоп-слов не найден: {args.StopWordsFile}")
            : Result.Ok(args);
    }

    public static Result<ProgramArgs> ValidateFont(ProgramArgs args)
    {
        var fonts = new InstalledFontCollection();

        var exists = fonts.Families
            .Any(f => string.Equals(f.Name, args.FontName, StringComparison.OrdinalIgnoreCase));

        return !exists ? Result.Fail<ProgramArgs>($"Шрифт не найден в системе: {args.FontName}") : Result.Ok(args);
    }
    
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