using FluentAssertions;
using Xunit;

namespace Zooper.Fox.Tests;

/// <summary>
/// Unit tests for the <see cref="Unit"/> struct.
/// </summary>
public class UnitTests
{
    [Fact]
    public void Value_ShouldReturnUnitInstance()
    {
        // Act
        var unit = Unit.Value;

        // Assert
        // The assertion mainly verifies that we can retrieve the Unit value
        unit.Should().BeOfType<Unit>();
    }

    [Fact]
    public void MultipleInstances_ShouldBeEqual()
    {
        // Arrange
        var unit1 = Unit.Value;
        var unit2 = Unit.Value;
        var unit3 = new Unit();

        // Act & Assert
        unit1.Should().Be(unit2);
        unit1.Should().Be(unit3);
        unit2.Should().Be(unit3);
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValueForAllInstances()
    {
        // Arrange
        var unit1 = Unit.Value;
        var unit2 = Unit.Value;
        var unit3 = new Unit();

        // Act & Assert
        unit1.GetHashCode().Should().Be(unit2.GetHashCode());
        unit1.GetHashCode().Should().Be(unit3.GetHashCode());
    }

    [Fact]
    public void EqualsMethod_ShouldWorkCorrectly()
    {
        // Arrange
        var unit1 = Unit.Value;
        var unit2 = Unit.Value;
        object unitObj = Unit.Value;
        object nonUnitObj = new object();

        // Act & Assert
        unit1.Equals(unit2).Should().BeTrue();
        unit1.Equals(unitObj).Should().BeTrue();
        unit1.Equals(nonUnitObj).Should().BeFalse();
        unit1.Equals(null).Should().BeFalse();
    }
} 