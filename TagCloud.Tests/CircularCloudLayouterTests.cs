using System.Drawing;
using FakeItEasy;
using FluentAssertions;
using TagCloud.Abstractions;
using TagCloud.Extensions;
using TagCloud.Implementations;

namespace TagCloud.Tests;

[TestFixture]
public class CircularCloudLayouterTests
{
    [SetUp]
    public void SetUp()
    {
        var center = new Point(0, 0);
        var spiral = new ArchimedeanSpiral(center);
        var centerShifter = new CenterShifter();
        layouter = new CircularCloudLayouter(center, spiral, centerShifter);
    }

    private CircularCloudLayouter layouter;

    [TestCaseSource(nameof(GenerateInvalidSizes))]
    public void PutNextRectangle_ShouldFail_WhenSizeIsInvalid(Size size)
    {
        var result = layouter.PutNextRectangle(size);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("должен быть больше нуля");
    }

    [TestCaseSource(nameof(GenerateDifferentSizes))]
    public void PutNextRectangle_ShouldHaveCorrectSize(Size size)
    {
        var result = layouter.PutNextRectangle(size);

        result.IsSuccess.Should().BeTrue();
        var rect = result.GetValueOrThrow();

        rect.Width.Should().Be(size.Width);
        rect.Height.Should().Be(size.Height);
    }

    [TestCaseSource(nameof(GenerateSpiralPoints))]
    public void PutNextRectangle_ShouldPlaceRectanglesAtExpectedPoints(Point[] expectedPoints)
    {
        var fakeSpiral = A.Fake<ISpiral>();
        var queue = new Queue<Point>(expectedPoints);
        A.CallTo(() => fakeSpiral.GetNextPoint()).ReturnsLazily(() => queue.Dequeue());

        var fakeLayouter = new CircularCloudLayouter(
            new Point(0, 0),
            fakeSpiral,
            new CenterShifter()
        );

        foreach (var point in expectedPoints)
        {
            var result = fakeLayouter.PutNextRectangle(new Size(10, 10));
            result.IsSuccess.Should().BeTrue();
            var rect = result.GetValueOrThrow();
            rect.Center().Should().Be(point);        }

        fakeLayouter.Rectangles.Should().HaveCount(expectedPoints.Length);
    }

    [TestCaseSource(nameof(GenerateRectanglesCount))]
    public void PutNextRectangle_ShouldPlaceManyRectanglesWithoutIntersections(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var result = layouter.PutNextRectangle(new Size(10, 10));
            result.IsSuccess.Should().BeTrue();
        }
        
        layouter.Rectangles.Should().HaveCount(count);

        foreach (var r1 in layouter.Rectangles)
        foreach (var r2 in layouter.Rectangles.Where(r2 => r1 != r2))
            r1.IntersectsWith(r2).Should().BeFalse();
    }

    [Test]
    public void PutNextRectangle_FirstRectangle_ShouldBeExactlyAtCenter()
    {
        var result = layouter.PutNextRectangle(new Size(10, 10));
        result.IsSuccess.Should().BeTrue();
        var rect = result.GetValueOrThrow();
        rect.Center().Should().Be(layouter.Center);
    }

    [Test]
    public void PutNextRectangle_ShouldCallCenterShifter()
    {
        var fakeShifter = A.Fake<ICenterShifter>();
        var fakeSpiral = A.Fake<ISpiral>();
        A.CallTo(() => fakeSpiral.GetNextPoint()).Returns(new Point(0, 0));
        var layouterWithFake = new CircularCloudLayouter(
            new Point(0, 0),
            fakeSpiral,
            fakeShifter
        );

        layouterWithFake.PutNextRectangle(new Size(10, 10));

        A.CallTo(() => fakeShifter.ShiftToCenter(A<Rectangle>._, A<Point>._, A<List<Rectangle>>._))
            .MustHaveHappened();
    }

    [Test]
    public void PutNextRectangle_ShouldTryMultipleSpiralPointsUntilFree()
    {
        var fakeSpiral = A.Fake<ISpiral>();
        var fakeShifter = new CenterShifter();

        var occupiedPoint = new Point(0, 0);
        var freePoint = new Point(10, 10);
        var callCount = 0;

        A.CallTo(() => fakeSpiral.GetNextPoint()).ReturnsLazily(() =>
        {
            callCount++;
            return callCount == 1 ? occupiedPoint : freePoint;
        });

        var layouterWithFake = new CircularCloudLayouter(
            new Point(0, 0),
            fakeSpiral,
            fakeShifter
        );

        var firstResult = layouterWithFake.PutNextRectangle(new Size(10, 10));
        firstResult.IsSuccess.Should().BeTrue(); 
        var firstRect = firstResult.GetValueOrThrow();

        var secondResult = layouterWithFake.PutNextRectangle(new Size(10, 10));
        secondResult.IsSuccess.Should().BeTrue();
        var secondRect = secondResult.GetValueOrThrow();

        secondRect.Center().Should().Be(freePoint);
    }

    public static IEnumerable<TestCaseData> GenerateRectanglesCount()
    {
        yield return new TestCaseData(1)
            .SetName("PutNextRectangle_SingleRectangle_NoIntersection")
            .SetDescription("Placing a single rectangle should succeed without intersections.");
        yield return new TestCaseData(2)
            .SetName("PutNextRectangle_TwoRectangles_NoIntersection")
            .SetDescription("Placing two rectangles should succeed without intersections.");
        yield return new TestCaseData(20)
            .SetName("PutNextRectangle_TwentyRectangles_NoIntersection")
            .SetDescription("Placing 20 rectangles should succeed without intersections.");
        yield return new TestCaseData(200)
            .SetName("PutNextRectangle_TwoHundredRectangles_NoIntersection")
            .SetDescription("Placing 200 rectangles should succeed without intersections.");
        for (var i = 50; i <= 200; i *= 2)
            yield return new TestCaseData(i)
                .SetName($"PutNextRectangle_{i}Rectangles_NoOverlap")
                .SetDescription($"Placing {i} rectangles should not produce overlaps.");
    }

    public static IEnumerable<TestCaseData> GenerateSpiralPoints()
    {
        var points = Enumerable.Range(0, 20).Select(i => new Point(i * 10, i * 10)).ToArray();
        yield return new TestCaseData(points)
            .SetName("PutNextRectangle_ShouldTryMultipleSpiralPointsUntilFree")
            .SetDescription("Rectangles should be placed at expected points along the spiral.");
    }

    public static IEnumerable<TestCaseData> GenerateDifferentSizes()
    {
        yield return new TestCaseData(new Size(10, 10))
            .SetName("PutNextRectangle_Size10x10")
            .SetDescription("Rectangle with width=10, height=10 should be placed correctly.");
        yield return new TestCaseData(new Size(20, 5))
            .SetName("PutNextRectangle_Size20x5")
            .SetDescription("Rectangle with width=20, height=5 should be placed correctly.");
        yield return new TestCaseData(new Size(5, 30))
            .SetName("PutNextRectangle_Size5x30")
            .SetDescription("Rectangle with width=5, height=30 should be placed correctly.");
        yield return new TestCaseData(new Size(15, 25))
            .SetName("PutNextRectangle_Size15x25")
            .SetDescription("Rectangle with width=15, height=25 should be placed correctly.");
    }

    public static IEnumerable<TestCaseData> GenerateInvalidSizes()
    {
        yield return new TestCaseData(new Size(0, 10))
            .SetName("PutNextRectangle_Throws_WhenWidthIsZero")
            .SetDescription("CanvasWidth is zero, should throw ArgumentException.");
        yield return new TestCaseData(new Size(10, 0))
            .SetName("PutNextRectangle_Throws_WhenHeightIsZero")
            .SetDescription("CanvasHeight is zero, should throw ArgumentException.");
        yield return new TestCaseData(new Size(-5, 10))
            .SetName("PutNextRectangle_Throws_WhenWidthIsNegative")
            .SetDescription("CanvasWidth is negative, should throw ArgumentException.");
        yield return new TestCaseData(new Size(10, -5))
            .SetName("PutNextRectangle_Throws_WhenHeightIsNegative")
            .SetDescription("CanvasHeight is negative, should throw ArgumentException.");
        yield return new TestCaseData(new Size(-5, -5))
            .SetName("PutNextRectangle_Throws_WhenWidthAndHeightNegative")
            .SetDescription("CanvasWidth and height are negative, should throw ArgumentException.");
    }
}