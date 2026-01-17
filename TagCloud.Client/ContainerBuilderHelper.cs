using Autofac;
using ErrorHandling;
using TagCloud.DI;

namespace TagCloud.Client;

public static class ContainerBuilderHelper
{
    public static Result<(IContainer container, ProgramArgs args)> BuildContainer(ProgramArgs args)
    {
        return Result.Of(() =>
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new TagCloudModule(args.WordsFile, args.StopWordsFile));

            var container = builder.Build();
            return (container, args);
        }, "Ошибка при создании контейнера");
    }
}