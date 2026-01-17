using System.Drawing;
using Autofac;
using Autofac.Features.AttributeFilters;
using TagCloud.Abstractions;
using TagCloud.Abstractions.Generic;
using TagCloud.Abstractions.WordsSource;
using TagCloud.Implementations;
using TagCloud.Implementations.Generics;
using TagCloud.Implementations.StopWords;
using TagCloud.Implementations.WordSource;

namespace TagCloud.DI;

public class TagCloudModule(string wordsFile, string stopWordsFile) : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var center = new Point(0, 0);

        builder.Register(_ => new ArchimedeanSpiral(center))
            .As<ISpiral>()
            .SingleInstance();

        builder.RegisterType<CenterShifter>()
            .As<ICenterShifter>()
            .SingleInstance();

        builder.Register(ctx =>
        {
            var spiral = ctx.Resolve<ISpiral>();
            var shifter = ctx.Resolve<ICenterShifter>();
            return new CircularCloudLayouter(center, spiral, shifter);
        })
        .As<CircularCloudLayouterBase>()
        .SingleInstance();

        builder.Register(ctx =>
            {
                var factory = ctx.Resolve<ISourceFactory<IWordsSource>>();
                return factory
                    .Create(wordsFile)
                    .GetValueOrThrow();
            })
            .Keyed<IWordsSource>("words")
            .SingleInstance();


        builder.Register(ctx =>
            {
                var factory = ctx.Resolve<ISourceFactory<IWordsSource>>();
                return factory
                    .Create(stopWordsFile)
                    .GetValueOrThrow();
            })
            .Keyed<IWordsSource>("stopWords")
            .SingleInstance();

        
        builder.Register<Func<ITagPlacer>>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return () => c.Resolve<ITagPlacer>();
            })
            .As<Func<ITagPlacer>>()
            .InstancePerDependency();

        builder.RegisterAssemblyTypes(typeof(TxtWordsSourceProvider).Assembly)
            .AssignableTo<IWordsProvider<IWordsSource>>()
            .As<IWordsProvider<IWordsSource>>()
            .SingleInstance();

        builder.RegisterType<SourceFactory<IWordsSource>>()
            .As<ISourceFactory<IWordsSource>>()
            .SingleInstance();

        builder.RegisterType<LowercasePreprocessor>()
            .As<IWordPreprocessor>()
            .SingleInstance();

        builder.RegisterType<StopWordsFilter>()
            .As<IWordFilter>()
            .WithAttributeFiltering() 
            .InstancePerDependency();

        builder.RegisterType<FrequencyCalculator>()
            .As<IFrequencyCalculator>()
            .SingleInstance();

        builder.RegisterType<LinearFontSizeMapper>()
            .As<IFontSizeMapper>()
            .SingleInstance();

        builder.RegisterType<TagCloudRenderer>()
            .As<ITagCloudRenderer>()
            .SingleInstance();

        builder.RegisterType<CircularTagPlacer>()
            .As<ITagPlacer>()
            .InstancePerDependency();

        builder.RegisterType<TagCloudGenerator>()
            .WithAttributeFiltering()
            .InstancePerDependency();
    }
}
