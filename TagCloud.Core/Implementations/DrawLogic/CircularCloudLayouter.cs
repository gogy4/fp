using System.Drawing;
using ErrorHandling;
using TagCloud.Abstractions;
using TagCloud.Extensions;

namespace TagCloud.Implementations;

public class CircularCloudLayouter(Point center, ISpiral spiral, ICenterShifter centerShifter, int maxAttempts = 1000)
    : CircularCloudLayouterBase(center)
{
    public override Result<Rectangle> PutNextRectangle(Size size)
    {
        return
            ValidateSize(size)
                .Then(FindFreeRectangle)
                .Then(rect =>
                    Result.Of(
                        () => centerShifter.ShiftToCenter(rect, Center, Rectangles),
                        "Ошибка при смещении прямоугольника к центру"))
                .Then(rect =>
                {
                    AddRectangle(rect);
                    return rect;
                });
    }

    private Result<Rectangle> FindFreeRectangle(Size size)
    {
        for (var i = 0; i < maxAttempts; i++)
        {
            var point = spiral.GetNextPoint();
            var candidate = CreateRectangleCenteredAt(point, size);

            if (!candidate.Intersects(Rectangles))
                return Result.Ok(candidate);
        }

        return Result.Fail<Rectangle>(
            $"Не удалось разместить прямоугольник {size.Width}x{size.Height} за {maxAttempts} попыток");
    }

    private static Rectangle CreateRectangleCenteredAt(Point center, Size size)
    {
        return new Rectangle(center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height);
    }
    
    private static Result<Size> ValidateSize(Size size)
    {
        return size.Width <= 0 || size.Height <= 0
            ? Result.Fail<Size>("Размер прямоугольника должен быть больше нуля")
            : Result.Ok(size);
    }
}