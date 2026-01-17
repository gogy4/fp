using ErrorHandling;
using TagCloud.Client;

public class Program
{
    public static int Main(string[] args)
    {
        return Run(args)
            .OnFail(err => Console.WriteLine("Ошибка: " + err))
            .IsSuccess
            ? 0
            : -1;
    }
    
    private static Result<None> Run(string[] args)
    {
        return
            ProgramArgsValidator.ValidateArgs(args)
                .Then(ProgramArgsValidator.ParseArgs)
                .Then(ProgramArgsValidator.ValidateFiles)
                .Then(ProgramArgsValidator.ValidateFont)
                .Then(ContainerBuilderHelper.BuildContainer)
                .Then(TagCloudGeneratorHelper.GenerateTagCloud)
                .Then(_ =>
                {
                    Console.WriteLine("Генерация завершена");
                    return None.Value;
                });
    }
}
