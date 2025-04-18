using FluentAssertions;
using Xunit;

namespace Zooper.Fox.Tests;

/// <summary>
/// Unit tests for the <see cref="EitherExtensions"/> class.
/// </summary>
public class EitherExtensionsTests
{
    #region TryGetRight Method

    [Fact]
    public void TryGetRight_WhenContainsRightValue_ShouldReturnTrueAndSetOutParameter()
    {
        // Arrange
        const int successValue = 42;
        var either = Either<string, int>.FromRight(successValue);

        // Act
        var result = either.TryGetRight(out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(successValue);
    }

    [Fact]
    public void TryGetRight_WhenContainsLeftValue_ShouldReturnFalseAndSetOutParameterToDefault()
    {
        // Arrange
        var either = Either<string, int>.FromLeft("Error occurred");

        // Act
        var result = either.TryGetRight(out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().Be(default);
    }

    [Fact]
    public void TryGetRight_WithNullableType_WhenContainsLeftValue_ShouldReturnDefaultNull()
    {
        // Arrange
        var either = Either<string, string>.FromLeft("Error occurred");

        // Act
        var result = either.TryGetRight(out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().BeNull();
    }

    #endregion

    #region TryGetLeft Method

    [Fact]
    public void TryGetLeft_WhenContainsLeftValue_ShouldReturnTrueAndSetOutParameter()
    {
        // Arrange
        const string errorMessage = "Error occurred";
        var either = Either<string, int>.FromLeft(errorMessage);

        // Act
        var result = either.TryGetLeft(out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(errorMessage);
    }

    [Fact]
    public void TryGetLeft_WhenContainsRightValue_ShouldReturnFalseAndSetOutParameterToDefault()
    {
        // Arrange
        var either = Either<string, int>.FromRight(42);

        // Act
        var result = either.TryGetLeft(out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().Be(default);
    }

    [Fact]
    public void TryGetLeft_WithValueType_WhenContainsRightValue_ShouldReturnDefaultValue()
    {
        // Arrange
        var either = Either<int, string>.FromRight("Success");

        // Act
        var result = either.TryGetLeft(out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().Be(0); // Default value for int
    }

    #endregion

    #region MapLeft Method

    [Fact]
    public void MapLeft_WhenContainsLeftValue_ShouldTransformLeftValue()
    {
        // Arrange
        const string errorMessage = "Error occurred";
        var either = Either<string, int>.FromLeft(errorMessage);

        // Act
        var result = either.MapLeft(msg => $"Error: {msg}");

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be($"Error: {errorMessage}");
    }

    [Fact]
    public void MapLeft_WhenContainsRightValue_ShouldReturnNewEitherWithSameRightValue()
    {
        // Arrange
        const int successValue = 42;
        var either = Either<string, int>.FromRight(successValue);

        // Act
        var result = either.MapLeft(msg => $"Error: {msg}");

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be(successValue);
    }

    [Fact]
    public void MapLeft_ShouldAllowChangingLeftType()
    {
        // Arrange
        const string errorMessage = "Error occurred";
        var either = Either<string, int>.FromLeft(errorMessage);

        // Act
        Either<int, int> result = either.MapLeft(msg => msg.Length);

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be(errorMessage.Length);
    }

    #endregion

    #region MapRight Method

    [Fact]
    public void MapRight_WhenContainsRightValue_ShouldTransformRightValue()
    {
        // Arrange
        const int successValue = 42;
        var either = Either<string, int>.FromRight(successValue);

        // Act
        var result = either.MapRight(val => val * 2);

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be(successValue * 2);
    }

    [Fact]
    public void MapRight_WhenContainsLeftValue_ShouldReturnNewEitherWithSameLeftValue()
    {
        // Arrange
        const string errorMessage = "Error occurred";
        var either = Either<string, int>.FromLeft(errorMessage);

        // Act
        var result = either.MapRight(val => val * 2);

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be(errorMessage);
    }

    [Fact]
    public void MapRight_ShouldAllowChangingRightType()
    {
        // Arrange
        const int successValue = 42;
        var either = Either<string, int>.FromRight(successValue);

        // Act
        Either<string, string> result = either.MapRight(val => val.ToString());

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be(successValue.ToString());
    }

    #endregion

    #region Bind Method

    [Fact]
    public void Bind_WhenContainsRightValue_ShouldApplyBinderFunction()
    {
        // Arrange
        const int successValue = 42;
        var either = Either<string, int>.FromRight(successValue);
        Func<int, Either<string, string>> binder = val => 
            val > 0 
                ? Either<string, string>.FromRight(val.ToString()) 
                : Either<string, string>.FromLeft("Value must be positive");

        // Act
        var result = either.Bind(binder);

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be(successValue.ToString());
    }

    [Fact]
    public void Bind_WhenContainsLeftValue_ShouldReturnNewEitherWithSameLeftValue()
    {
        // Arrange
        const string errorMessage = "Error occurred";
        var either = Either<string, int>.FromLeft(errorMessage);
        Func<int, Either<string, string>> binder = val => 
            val > 0 
                ? Either<string, string>.FromRight(val.ToString()) 
                : Either<string, string>.FromLeft("Value must be positive");

        // Act
        var result = either.Bind(binder);

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be(errorMessage);
    }

    [Fact]
    public void Bind_WhenBinderReturnsLeft_ShouldReturnNewLeftEither()
    {
        // Arrange
        const int negativeValue = -5;
        var either = Either<string, int>.FromRight(negativeValue);
        Func<int, Either<string, string>> binder = val => 
            val > 0 
                ? Either<string, string>.FromRight(val.ToString()) 
                : Either<string, string>.FromLeft("Value must be positive");

        // Act
        var result = either.Bind(binder);

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be("Value must be positive");
    }

    #endregion

    #region Chaining Multiple Operations

    [Fact]
    public void ChainingExtensionMethods_ShouldComposeCorrectly()
    {
        // Arrange
        const int initialValue = 10;
        var either = Either<string, int>.FromRight(initialValue);

        // Act
        var result = either
            .MapRight(val => val * 2)  // 20
            .Bind(val => val > 15 
                ? Either<string, int>.FromRight(val + 5)  // 25
                : Either<string, int>.FromLeft("Value too small"))
            .MapRight(val => val.ToString());

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be("25");
    }

    [Fact]
    public void ChainingExtensionMethods_WhenIntermediateOperationFails_ShouldShortCircuit()
    {
        // Arrange
        const int initialValue = 5;
        var either = Either<string, int>.FromRight(initialValue);

        // Act
        var result = either
            .MapRight(val => val * 2)  // 10
            .Bind(val => val > 15 
                ? Either<string, int>.FromRight(val + 5)
                : Either<string, int>.FromLeft("Value too small"))
            .MapRight(val => val.ToString());

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be("Value too small");
    }

    #endregion
} 