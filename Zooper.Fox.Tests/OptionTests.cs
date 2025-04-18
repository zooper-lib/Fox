using FluentAssertions;
using Xunit;

namespace Zooper.Fox.Tests;

/// <summary>
/// Unit tests for the <see cref="Option{T}"/> class.
/// </summary>
public class OptionTests
{
    #region Factory Methods

    [Fact]
    public void Some_ShouldCreateOptionWithValue()
    {
        // Arrange
        const string value = "test value";

        // Act
        var option = Option<string>.Some(value);

        // Assert
        option.IsSome.Should().BeTrue();
        option.IsNone.Should().BeFalse();
        option.Value.Should().Be(value);
    }

    [Fact]
    public void None_ShouldCreateEmptyOption()
    {
        // Act
        var option = Option<string>.None();

        // Assert
        option.IsNone.Should().BeTrue();
        option.IsSome.Should().BeFalse();
    }

    [Fact]
    public void Some_WithNullValue_ShouldCreateOptionWithNullValue()
    {
        // Act
        var option = Option<string>.Some(null!);

        // Assert
        option.IsSome.Should().BeTrue();
        option.Value.Should().BeNull();
    }

    #endregion

    #region Property Access

    [Fact]
    public void Value_WhenIsSome_ShouldReturnValue()
    {
        // Arrange
        const string value = "test value";
        var option = Option<string>.Some(value);

        // Act
        var result = option.Value;

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void Value_WhenIsNone_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var option = Option<string>.None();

        // Act & Assert
        option.Invoking(o => _ = o.Value)
              .Should().Throw<InvalidOperationException>()
              .WithMessage("Cannot access Value on None.");
    }

    #endregion

    #region Match Method

    [Fact]
    public void Match_WhenIsSome_ShouldInvokeSomeFunc()
    {
        // Arrange
        const int value = 42;
        var option = Option<int>.Some(value);

        // Act
        var result = option.Match(
            someValue => $"Value: {someValue}",
            () => "No value"
        );

        // Assert
        result.Should().Be($"Value: {value}");
    }

    [Fact]
    public void Match_WhenIsNone_ShouldInvokeNoneFunc()
    {
        // Arrange
        var option = Option<int>.None();

        // Act
        var result = option.Match(
            someValue => $"Value: {someValue}",
            () => "No value"
        );

        // Assert
        result.Should().Be("No value");
    }

    [Fact]
    public void Match_WithComplexReturnType_ShouldWork()
    {
        // Arrange
        var person = new Person { Name = "John", Age = 30 };
        var option = Option<Person>.Some(person);

        // Act
        var result = option.Match(
            p => new PersonViewModel { FullName = p.Name, IsAdult = p.Age >= 18 },
            () => new PersonViewModel { FullName = "Guest", IsAdult = false }
        );

        // Assert
        result.FullName.Should().Be(person.Name);
        result.IsAdult.Should().BeTrue();
    }

    #endregion

    #region Integration with Either

    [Fact]
    public void Option_IsCompatibleWithEitherMethods()
    {
        // Arrange
        var option = Option<string>.Some("test");

        // Act & Assert - verify that Option is an Either under the hood
        ((Either<Unit, string>)option).IsRight.Should().BeTrue();
        option.IsSome.Should().BeTrue();
    }

    [Fact]
    public void Option_CanUseEitherMatchMethod()
    {
        // Arrange
        var option = Option<int>.Some(42);

        // Act - Using the Either.Match method
        var result = ((Either<Unit, int>)option).Match(
            _ => "None",
            value => $"Some: {value}"
        );

        // Assert
        result.Should().Be("Some: 42");
    }

    #endregion

    #region Working with Value Types and Reference Types

    [Fact]
    public void Option_WithValueType_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var option = Option<int>.Some(42);

        // Assert
        option.IsSome.Should().BeTrue();
        option.Value.Should().Be(42);
    }

    [Fact]
    public void Option_WithReferenceType_ShouldWorkCorrectly()
    {
        // Arrange
        var person = new Person { Name = "John", Age = 30 };
        
        // Act
        var option = Option<Person>.Some(person);

        // Assert
        option.IsSome.Should().BeTrue();
        option.Value.Should().BeSameAs(person);
    }

    #endregion

    #region Practical Usage Examples

    [Fact]
    public void Example_NullableToOption_ShouldConvertCorrectly()
    {
        // Arrange
        string? nullableValue1 = "test";
        string? nullableValue2 = null;

        // Act
        var option1 = NullableToOption(nullableValue1);
        var option2 = NullableToOption(nullableValue2);

        // Assert
        option1.IsSome.Should().BeTrue();
        option1.Value.Should().Be("test");
        
        option2.IsNone.Should().BeTrue();
    }

    [Fact]
    public void Example_TryGetValueToOption_ShouldConvertCorrectly()
    {
        // Arrange
        var dictionary = new System.Collections.Generic.Dictionary<string, int>
        {
            ["existing"] = 42
        };

        // Act
        var option1 = TryGetValueToOption(dictionary, "existing");
        var option2 = TryGetValueToOption(dictionary, "nonexisting");

        // Assert
        option1.IsSome.Should().BeTrue();
        option1.Value.Should().Be(42);
        
        option2.IsNone.Should().BeTrue();
    }

    #endregion

    #region Helper Methods

    // Helper method for converting nullable to Option
    private static Option<T> NullableToOption<T>(T? value) where T : class
    {
        return value != null
            ? Option<T>.Some(value)
            : Option<T>.None();
    }

    // Helper method for converting TryGetValue pattern to Option
    private static Option<TValue> TryGetValueToOption<TKey, TValue>(
        System.Collections.Generic.Dictionary<TKey, TValue> dictionary, 
        TKey key) where TKey : notnull
    {
        return dictionary.TryGetValue(key, out var value)
            ? Option<TValue>.Some(value)
            : Option<TValue>.None();
    }

    #endregion

    #region Test Classes

    private class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private class PersonViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsAdult { get; set; }
    }

    #endregion
} 