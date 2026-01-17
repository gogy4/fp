using System;

namespace ErrorHandling.QuerySyntax;

public static class ResultQueryExpressionExtensions
{
    public static Result<TOutput> SelectMany<TInput, TOutput>(
        this Result<TInput> input,
        Func<TInput, Result<TOutput>> continuation)
    {
        return input.Then(continuation);
    }

    public static Result<TSelected> SelectMany<TInput, TOutput, TSelected>(
        this Result<TInput> input,
        Func<TInput, Result<TOutput>> continuation,
        Func<TInput, TOutput, TSelected> resultSelector)
    {
        var tempResult = input.SelectMany(continuation);
        return !tempResult.IsSuccess 
            ? Result.Fail<TSelected>(tempResult.Error) 
            : Result.Of(() => resultSelector(input.Value, tempResult.Value));
    }
}