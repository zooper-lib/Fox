using FluentAssertions;
using Moq;
using Xunit;

namespace Zooper.Fox.Tests;

/// <summary>
/// Unit tests for the <see cref="Either{TLeft,TRight}"/> class.
/// </summary>
public class EitherTests
{
    #region Constructor and Factory Methods

    [Fact]
    public void FromLeft_ShouldCreateInstanceWithLeftValue()
    {
        // Arrange
        const string errorMessage = "Error occurred";

        // Act
        var either = Either<string, int>.FromLeft(errorMessage);

        // Assert
        either.IsLeft.Should().BeTrue();
        either.IsRight.Should().BeFalse();
        either.Left.Should().Be(errorMessage);
    }

    [Fact]
    public void FromRight_ShouldCreateInstanceWithRightValue()
    {
        // Arrange
        const int successValue = 42;

        // Act
        var either = Either<string, int>.FromRight(successValue);

        // Assert
        either.IsRight.Should().BeTrue();
        either.IsLeft.Should().BeFalse();
        either.Right.Should().Be(successValue);
    }

    #endregion

    #region Property Access

    [Fact]
    public void Left_WhenContainsLeftValue_ShouldReturnValue()
    {
        // Arrange
        const string errorMessage = "Error occurred";
        var either = Either<string, int>.FromLeft(errorMessage);

        // Act
        var result = either.Left;

        // Assert
        result.Should().Be(errorMessage);
    }

    [Fact]
    public void Left_WhenContainsRightValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var either = Either<string, int>.FromRight(42);

        // Act & Assert
        either.Invoking(e => _ = e.Left)
              .Should().Throw<InvalidOperationException>()
              .WithMessage("Cannot access Left when IsLeft is false.");
    }

    [Fact]
    public void Right_WhenContainsRightValue_ShouldReturnValue()
    {
        // Arrange
        const int successValue = 42;
        var either = Either<string, int>.FromRight(successValue);

        // Act
        var result = either.Right;

        // Assert
        result.Should().Be(successValue);
    }

    [Fact]
    public void Right_WhenContainsLeftValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var either = Either<string, int>.FromLeft("Error occurred");

        // Act & Assert
        either.Invoking(e => _ = e.Right)
              .Should().Throw<InvalidOperationException>()
              .WithMessage("Cannot access Right when IsRight is false.");
    }

    #endregion

    #region Implicit Conversions

    [Fact]
    public void ImplicitConversion_FromLeftValue_ShouldCreateEitherWithLeftValue()
    {
        // Arrange
        const string errorMessage = "Error occurred";

        // Act
        Either<string, int> either = errorMessage;

        // Assert
        either.IsLeft.Should().BeTrue();
        either.Left.Should().Be(errorMessage);
    }

    [Fact]
    public void ImplicitConversion_FromRightValue_ShouldCreateEitherWithRightValue()
    {
        // Arrange
        const int successValue = 42;

        // Act
        Either<string, int> either = successValue;

        // Assert
        either.IsRight.Should().BeTrue();
        either.Right.Should().Be(successValue);
    }

    #endregion

    #region Match Method with Function

    [Fact]
    public void Match_WhenContainsLeftValue_ShouldInvokeLeftFunction()
    {
        // Arrange
        const string errorMessage = "Error occurred";
        var either = Either<string, int>.FromLeft(errorMessage);
        
        // Act
        var result = either.Match(
            left => $"Error: {left}",
            right => $"Success: {right}"
        );

        // Assert
        result.Should().Be($"Error: {errorMessage}");
    }

    [Fact]
    public void Match_WhenContainsRightValue_ShouldInvokeRightFunction()
    {
        // Arrange
        const int successValue = 42;
        var either = Either<string, int>.FromRight(successValue);
        
        // Act
        var result = either.Match(
            left => $"Error: {left}",
            right => $"Success: {right}"
        );

        // Assert
        result.Should().Be($"Success: {successValue}");
    }

    [Fact]
    public void Match_WithFunctions_ShouldPassCorrectValueToHandler()
    {
        // Arrange
        var leftMock = new Mock<Func<string, string>>();
        var rightMock = new Mock<Func<int, string>>();
        const string errorMessage = "Error occurred";
        var either = Either<string, int>.FromLeft(errorMessage);

        // Act
        either.Match(leftMock.Object, rightMock.Object);

        // Assert
        leftMock.Verify(f => f(errorMessage), Times.Once);
        rightMock.Verify(f => f(It.IsAny<int>()), Times.Never);
    }

    #endregion

    #region Match Method with Action

    [Fact]
    public void Match_WithActions_WhenContainsLeftValue_ShouldInvokeLeftAction()
    {
        // Arrange
        var leftActionCalled = false;
        var rightActionCalled = false;
        const string errorMessage = "Error occurred";
        var either = Either<string, int>.FromLeft(errorMessage);

        // Act
        either.Match(
            left => { leftActionCalled = true; },
            right => { rightActionCalled = true; }
        );

        // Assert
        leftActionCalled.Should().BeTrue();
        rightActionCalled.Should().BeFalse();
    }

    [Fact]
    public void Match_WithActions_WhenContainsRightValue_ShouldInvokeRightAction()
    {
        // Arrange
        var leftActionCalled = false;
        var rightActionCalled = false;
        const int successValue = 42;
        var either = Either<string, int>.FromRight(successValue);

        // Act
        either.Match(
            left => { leftActionCalled = true; },
            right => { rightActionCalled = true; }
        );

        // Assert
        leftActionCalled.Should().BeFalse();
        rightActionCalled.Should().BeTrue();
    }

    [Fact]
    public void Match_WithActions_ShouldPassCorrectValueToHandler()
    {
        // Arrange
        var leftMock = new Mock<Action<string>>();
        var rightMock = new Mock<Action<int>>();
        const int successValue = 42;
        var either = Either<string, int>.FromRight(successValue);

        // Act
        either.Match(leftMock.Object, rightMock.Object);

        // Assert
        leftMock.Verify(f => f(It.IsAny<string>()), Times.Never);
        rightMock.Verify(f => f(successValue), Times.Once);
    }

    #endregion

    #region Null Values

    [Fact]
    public void FromLeft_WithNullValue_ShouldCreateValidInstance()
    {
        // Arrange & Act
        var either = Either<string, int>.FromLeft(null!);

        // Assert
        either.IsLeft.Should().BeTrue();
        either.Left.Should().BeNull();
    }

    [Fact]
    public void FromRight_WithNullValue_ShouldCreateValidInstance()
    {
        // Arrange & Act
        var either = Either<string, string>.FromRight(null!);

        // Assert
        either.IsRight.Should().BeTrue();
        either.Right.Should().BeNull();
    }

    #endregion

    #region Reference and Value Types

    [Fact]
    public void Either_WithValueTypeLeft_ShouldWorkCorrectly()
    {
        // Arrange
        const int errorCode = 404;

        // Act
        var either = Either<int, string>.FromLeft(errorCode);

        // Assert
        either.IsLeft.Should().BeTrue();
        either.Left.Should().Be(errorCode);
    }

    [Fact]
    public void Either_WithComplexReferenceType_ShouldWorkCorrectly()
    {
        // Arrange
        var person = new Person { Name = "John", Age = 30 };

        // Act
        var either = Either<string, Person>.FromRight(person);

        // Assert
        either.IsRight.Should().BeTrue();
        either.Right.Should().BeSameAs(person);
    }

    #endregion

    // Helper class for testing with complex types
    private class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
} 