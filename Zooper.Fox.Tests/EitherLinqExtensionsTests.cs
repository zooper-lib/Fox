using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Zooper.Fox.Tests;

/// <summary>
/// Unit tests for the <see cref="EitherLinqExtensions"/> class.
/// </summary>
public class EitherLinqExtensionsTests
{
    #region Select Tests

    [Fact]
    public void Select_WhenContainsRightValue_ShouldTransformRightValue()
    {
        // Arrange
        var either = Either<string, int>.FromRight(42);
        
        // Act
        var result = either.Select(x => x.ToString());

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be("42");
    }

    [Fact]
    public void Select_WhenContainsLeftValue_ShouldNotTransformAndPreserveLeftValue()
    {
        // Arrange
        const string errorMessage = "Error occurred";
        var either = Either<string, int>.FromLeft(errorMessage);
        
        // Act
        var result = either.Select(x => x.ToString());

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be(errorMessage);
    }

    [Fact]
    public void Select_WithComplexTransformation_ShouldApplyTransformation()
    {
        // Arrange
        var person = new Person { Name = "John", Age = 30 };
        var either = Either<string, Person>.FromRight(person);
        
        // Act
        var result = either.Select(p => new { FullName = p.Name, YearOfBirth = DateTime.Now.Year - p.Age });

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.FullName.Should().Be("John");
        result.Right.YearOfBirth.Should().Be(DateTime.Now.Year - 30);
    }

    #endregion

    #region Where Tests

    [Fact]
    public void Where_WhenRightValueSatisfiesPredicate_ShouldReturnOriginalEither()
    {
        // Arrange
        var either = Either<string, int>.FromRight(42);
        
        // Act
        var result = either.Where(x => x > 0);

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be(42);
    }

    [Fact]
    public void Where_WhenRightValueDoesNotSatisfyPredicate_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var either = Either<string, int>.FromRight(-5);
        
        // Act & Assert
        either.Invoking(e => e.Where(x => x > 0))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("Predicate returned false for Right value");
    }

    [Fact]
    public void Where_WhenContainsLeftValue_ShouldReturnOriginalEither()
    {
        // Arrange
        const string errorMessage = "Error occurred";
        var either = Either<string, int>.FromLeft(errorMessage);
        
        // Act
        var result = either.Where(x => x > 0); // Predicate should never be evaluated

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be(errorMessage);
    }

    #endregion

    #region SelectMany Tests

    [Fact]
    public void SelectMany_WhenBothEithersContainRightValues_ShouldCombineThemUsingResultSelector()
    {
        // Arrange
        var either = Either<string, int>.FromRight(42);
        
        // Act
        var result = either.SelectMany(
            x => Either<string, string>.FromRight($"Value: {x}"),
            (x, y) => $"{y} doubled is {x * 2}"
        );

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be("Value: 42 doubled is 84");
    }

    [Fact]
    public void SelectMany_WhenFirstEitherContainsLeftValue_ShouldReturnLeftValue()
    {
        // Arrange
        const string errorMessage = "First error";
        var either = Either<string, int>.FromLeft(errorMessage);
        
        // Act
        var result = either.SelectMany(
            x => Either<string, string>.FromRight($"Value: {x}"),
            (x, y) => $"{y} doubled is {x * 2}"
        );

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be(errorMessage);
    }

    [Fact]
    public void SelectMany_WhenSecondEitherContainsLeftValue_ShouldReturnLeftValue()
    {
        // Arrange
        const string errorMessage = "Second error";
        var either = Either<string, int>.FromRight(42);
        
        // Act
        var result = either.SelectMany(
            _ => Either<string, string>.FromLeft(errorMessage),
            (x, y) => $"{y} doubled is {x * 2}"
        );

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be(errorMessage);
    }

    [Fact]
    public void SelectMany_WithLinqQuerySyntax_ShouldWorkCorrectly()
    {
        // Arrange
        var user = Either<string, User>.FromRight(new User { Id = 1, Name = "John" });
        var order = Either<string, Order>.FromRight(new Order { UserId = 1, OrderId = 101 });

        // Act
        var result = from u in user
                     from o in order
                     where o.UserId == u.Id
                     select new UserOrder { UserName = u.Name, OrderId = o.OrderId };

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.UserName.Should().Be("John");
        result.Right.OrderId.Should().Be(101);
    }

    #endregion

    #region OrElse Tests

    [Fact]
    public void OrElse_WithDefaultValue_WhenContainsRightValue_ShouldReturnOriginalEither()
    {
        // Arrange
        var either = Either<string, int>.FromRight(42);
        
        // Act
        var result = either.OrElse(0);

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be(42);
    }

    [Fact]
    public void OrElse_WithDefaultValue_WhenContainsLeftValue_ShouldReturnEitherWithDefaultRightValue()
    {
        // Arrange
        var either = Either<string, int>.FromLeft("Error occurred");
        
        // Act
        var result = either.OrElse(0);

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be(0);
    }

    [Fact]
    public void OrElse_WithFactory_WhenContainsRightValue_ShouldReturnOriginalEither()
    {
        // Arrange
        var either = Either<string, int>.FromRight(42);
        
        // Act
        var result = either.OrElse(err => err.Length);

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be(42);
    }

    [Fact]
    public void OrElse_WithFactory_WhenContainsLeftValue_ShouldReturnEitherWithComputedRightValue()
    {
        // Arrange
        const string errorMessage = "Error occurred";
        var either = Either<string, int>.FromLeft(errorMessage);
        
        // Act
        var result = either.OrElse(err => err.Length);

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be(errorMessage.Length);
    }

    #endregion

    #region Zip Tests

    [Fact]
    public void Zip_WhenBothEithersContainRightValues_ShouldReturnTuple()
    {
        // Arrange
        var first = Either<string, int>.FromRight(42);
        var second = Either<string, string>.FromRight("Success");
        
        // Act
        var result = first.Zip(second);

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be((42, "Success"));
    }

    [Fact]
    public void Zip_WhenFirstEitherContainsLeftValue_ShouldReturnLeftValue()
    {
        // Arrange
        const string errorMessage = "First error";
        var first = Either<string, int>.FromLeft(errorMessage);
        var second = Either<string, string>.FromRight("Success");
        
        // Act
        var result = first.Zip(second);

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be(errorMessage);
    }

    [Fact]
    public void Zip_WhenSecondEitherContainsLeftValue_ShouldReturnLeftValue()
    {
        // Arrange
        const string errorMessage = "Second error";
        var first = Either<string, int>.FromRight(42);
        var second = Either<string, string>.FromLeft(errorMessage);
        
        // Act
        var result = first.Zip(second);

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be(errorMessage);
    }

    [Fact]
    public void Zip_WhenBothEithersContainLeftValues_ShouldReturnFirstLeftValue()
    {
        // Arrange
        const string firstError = "First error";
        const string secondError = "Second error";
        var first = Either<string, int>.FromLeft(firstError);
        var second = Either<string, string>.FromLeft(secondError);
        
        // Act
        var result = first.Zip(second);

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be(firstError); // First left value is returned
    }

    #endregion

    #region Flatten Tests

    [Fact]
    public void Flatten_WhenOuterEitherContainsRightValueWithInnerRightEither_ShouldFlattenToRightValue()
    {
        // Arrange
        var innerEither = Either<string, int>.FromRight(42);
        var outerEither = Either<string, Either<string, int>>.FromRight(innerEither);
        
        // Act
        var result = outerEither.Flatten();

        // Assert
        result.IsRight.Should().BeTrue();
        result.Right.Should().Be(42);
    }

    [Fact]
    public void Flatten_WhenOuterEitherContainsRightValueWithInnerLeftEither_ShouldFlattenToLeftValue()
    {
        // Arrange
        const string innerError = "Inner error";
        var innerEither = Either<string, int>.FromLeft(innerError);
        var outerEither = Either<string, Either<string, int>>.FromRight(innerEither);
        
        // Act
        var result = outerEither.Flatten();

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be(innerError);
    }

    [Fact]
    public void Flatten_WhenOuterEitherContainsLeftValue_ShouldReturnOuterLeftValue()
    {
        // Arrange
        const string outerError = "Outer error";
        var outerEither = Either<string, Either<string, int>>.FromLeft(outerError);
        
        // Act
        var result = outerEither.Flatten();

        // Assert
        result.IsLeft.Should().BeTrue();
        result.Left.Should().Be(outerError);
    }

    #endregion

    #region LeftToEnumerable Tests

    [Fact]
    public void LeftToEnumerable_WhenContainsLeftValue_ShouldReturnEnumerableWithOneElement()
    {
        // Arrange
        const string errorMessage = "Error occurred";
        var either = Either<string, int>.FromLeft(errorMessage);
        
        // Act
        var result = either.LeftToEnumerable().ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().Be(errorMessage);
    }

    [Fact]
    public void LeftToEnumerable_WhenContainsRightValue_ShouldReturnEmptyEnumerable()
    {
        // Arrange
        var either = Either<string, int>.FromRight(42);
        
        // Act
        var result = either.LeftToEnumerable().ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void LeftToEnumerable_WithCollectionOfEithers_ShouldWorkCorrectly()
    {
        // Arrange
        var eithers = new[]
        {
            Either<string, int>.FromRight(1),
            Either<string, int>.FromLeft("Error 1"),
            Either<string, int>.FromRight(2),
            Either<string, int>.FromLeft("Error 2"),
        };
        
        // Act
        var errors = eithers.SelectMany(e => e.LeftToEnumerable()).ToList();

        // Assert
        errors.Should().HaveCount(2);
        errors[0].Should().Be("Error 1");
        errors[1].Should().Be("Error 2");
    }

    #endregion

    #region RightToEnumerable Tests

    [Fact]
    public void RightToEnumerable_WhenContainsRightValue_ShouldReturnEnumerableWithOneElement()
    {
        // Arrange
        const int value = 42;
        var either = Either<string, int>.FromRight(value);
        
        // Act
        var result = either.RightToEnumerable().ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().Be(value);
    }

    [Fact]
    public void RightToEnumerable_WhenContainsLeftValue_ShouldReturnEmptyEnumerable()
    {
        // Arrange
        var either = Either<string, int>.FromLeft("Error occurred");
        
        // Act
        var result = either.RightToEnumerable().ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void RightToEnumerable_WithCollectionOfEithers_ShouldWorkCorrectly()
    {
        // Arrange
        var eithers = new[]
        {
            Either<string, int>.FromRight(1),
            Either<string, int>.FromLeft("Error 1"),
            Either<string, int>.FromRight(2),
            Either<string, int>.FromLeft("Error 2"),
        };
        
        // Act
        var values = eithers.SelectMany(e => e.RightToEnumerable()).ToList();

        // Assert
        values.Should().HaveCount(2);
        values[0].Should().Be(1);
        values[1].Should().Be(2);
    }

    #endregion

    #region Helper Classes for Testing

    private class Person
    {
        public string? Name { get; set; }
        public int Age { get; set; }
    }

    private class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private class Order
    {
        public int UserId { get; set; }
        public int OrderId { get; set; }
    }

    private class UserOrder
    {
        public string? UserName { get; set; }
        public int OrderId { get; set; }
    }

    #endregion
} 