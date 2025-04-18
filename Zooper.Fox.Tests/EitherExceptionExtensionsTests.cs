using FluentAssertions;
using Xunit;

namespace Zooper.Fox.Tests;

/// <summary>
/// Unit tests for the <see cref="EitherExceptionExtensions"/> class.
/// </summary>
public class EitherExceptionExtensionsTests
{
    #region Try Method

    [Fact]
    public void Try_WhenFunctionSucceeds_ShouldReturnRightWithResult()
    {
        // Arrange
        Func<int> successFunc = () => 42;

        // Act
        var result = EitherExceptionExtensions.Try(successFunc);

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be(42);
    }

    [Fact]
    public void Try_WhenFunctionThrows_ShouldReturnLeftWithException()
    {
        // Arrange
        Func<int> failingFunc = () => throw new InvalidOperationException("Test exception");

        // Act
        var result = EitherExceptionExtensions.Try(failingFunc);

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().BeOfType<InvalidOperationException>();
        result.Left.Message.Should().Be("Test exception");
    }

    [Fact]
    public void Try_WithDifferentExceptionTypes_ShouldCaptureCorrectly()
    {
        // Arrange
        Func<int> argumentFunc = () => throw new ArgumentException("Invalid argument");
        Func<int> nullRefFunc = () => throw new NullReferenceException("Null reference");

        // Act
        var result1 = EitherExceptionExtensions.Try(argumentFunc);
        var result2 = EitherExceptionExtensions.Try(nullRefFunc);

        // Assert
        result1.IsLeft.Should().BeTrue();
        result1.Left.Should().BeOfType<ArgumentException>();

        result2.IsLeft.Should().BeTrue();
        result2.Left.Should().BeOfType<NullReferenceException>();
    }

    #endregion

    #region TryAsync Method

    [Fact]
    public async Task TryAsync_WhenAsyncFunctionSucceeds_ShouldReturnRightWithResult()
    {
        // Arrange
        Func<Task<string>> successFunc = () => Task.FromResult("Success");

        // Act
        var result = await EitherExceptionExtensions.TryAsync(successFunc);

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be("Success");
    }

    [Fact]
    public async Task TryAsync_WhenAsyncFunctionThrows_ShouldReturnLeftWithException()
    {
        // Arrange
        Func<Task<string>> failingFunc = () => 
            Task.FromException<string>(new InvalidOperationException("Async test exception"));

        // Act
        var result = await EitherExceptionExtensions.TryAsync(failingFunc);

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().BeOfType<InvalidOperationException>();
        result.Left.Message.Should().Be("Async test exception");
    }

    [Fact]
    public async Task TryAsync_WithSynchronousExceptionInAsyncMethod_ShouldCaptureCorrectly()
    {
        // Arrange
        Func<Task<int>> failingFunc = () => { throw new ArgumentException("Sync exception in async method"); };

        // Act
        var result = await EitherExceptionExtensions.TryAsync(failingFunc);

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().BeOfType<ArgumentException>();
        result.Left.Message.Should().Be("Sync exception in async method");
    }

    #endregion

    #region Practical Usage Examples

    [Fact]
    public void Try_WithParsingOperation_ShouldHandleExceptionsGracefully()
    {
        // Arrange
        string validNumber = "42";
        string invalidNumber = "not a number";

        // Act
        var result1 = EitherExceptionExtensions.Try(() => int.Parse(validNumber));
        var result2 = EitherExceptionExtensions.Try(() => int.Parse(invalidNumber));

        // Assert
        result1.IsRight.Should().BeTrue();
        result1.Right.Should().Be(42);

        result2.IsLeft.Should().BeTrue();
        result2.Left.Should().BeOfType<FormatException>();
    }

    [Fact]
    public void Try_WithPotentiallyFailingOperation_ShouldAllowSafeHandling()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };

        // Act - Access valid and invalid indices
        var result1 = EitherExceptionExtensions.Try(() => array[1]);
        var result2 = EitherExceptionExtensions.Try(() => array[10]);

        // Assert
        result1.IsRight.Should().BeTrue();
        result1.Right.Should().Be(2);

        result2.IsLeft.Should().BeTrue();
        result2.Left.Should().BeOfType<IndexOutOfRangeException>();
    }

    #endregion
} 