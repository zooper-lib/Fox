using FluentAssertions;
using Xunit;

namespace Zooper.Fox.Tests;

/// <summary>
/// Unit tests for the <see cref="EitherAsyncExtensions"/> class.
/// </summary>
public class EitherAsyncExtensionsTests
{
	#region MapLeftAsync Method

	[Fact]
	public async Task MapLeftAsync_WhenContainsLeftValue_ShouldApplyAsyncMapperToLeft()
	{
		// Arrange
		const string errorMessage = "Error occurred";
		var either = Either<string, int>.FromLeft(errorMessage);

        static Task<int> AsyncMapper(string msg) => Task.FromResult(msg.Length);

		// Act
		var result = await either.MapLeftAsync(AsyncMapper);

		// Assert
		result.IsLeft.Should().BeTrue();
		result.Left.Should().Be(errorMessage.Length);
	}

	[Fact]
	public async Task MapLeftAsync_WhenContainsRightValue_ShouldNotApplyMapper()
	{
		// Arrange
		const int successValue = 42;
		var either = Either<string, int>.FromRight(successValue);

        static Task<int> AsyncMapper(string msg) => Task.FromResult(msg.Length);

		// Act
		var result = await either.MapLeftAsync(AsyncMapper);

		// Assert
		result.IsRight.Should().BeTrue();
		result.Right.Should().Be(successValue);
	}

	[Fact]
	public async Task MapLeftAsync_WithAsyncMapperThatThrows_ShouldPropagateException()
	{
		// Arrange
		const string errorMessage = "Error occurred";
		var either = Either<string, int>.FromLeft(errorMessage);

        static Task<int> ThrowingMapper(string _) => 
			throw new InvalidOperationException("Mapper exception");

		// Act & Assert
		await either.Invoking(e => e.MapLeftAsync(ThrowingMapper))
			.Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("Mapper exception");
	}

	#endregion

	#region MapRightAsync Method

	[Fact]
	public async Task MapRightAsync_WhenContainsRightValue_ShouldApplyAsyncMapperToRight()
	{
		// Arrange
		const int successValue = 42;
		var either = Either<string, int>.FromRight(successValue);

        static Task<string> AsyncMapper(int val) => Task.FromResult($"Value: {val}");

		// Act
		var result = await either.MapRightAsync(AsyncMapper);

		// Assert
		result.IsRight.Should().BeTrue();
		result.Right.Should().Be($"Value: {successValue}");
	}

	[Fact]
	public async Task MapRightAsync_WhenContainsLeftValue_ShouldNotApplyMapper()
	{
		// Arrange
		const string errorMessage = "Error occurred";
		var either = Either<string, int>.FromLeft(errorMessage);

        static Task<string> AsyncMapper(int val) => Task.FromResult($"Value: {val}");

		// Act
		var result = await either.MapRightAsync(AsyncMapper);

		// Assert
		result.IsLeft.Should().BeTrue();
		result.Left.Should().Be(errorMessage);
	}

	[Fact]
	public async Task MapRightAsync_WithAsyncMapperThatThrows_ShouldPropagateException()
	{
		// Arrange
		const int successValue = 42;
		var either = Either<string, int>.FromRight(successValue);

        static Task<string> ThrowingMapper(int _) => 
			throw new InvalidOperationException("Mapper exception");

		// Act & Assert
		await either.Invoking(e => e.MapRightAsync(ThrowingMapper))
			.Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("Mapper exception");
	}

	#endregion

	#region BindAsync Method

	[Fact]
	public async Task BindAsync_WhenContainsRightValue_ShouldApplyAsyncBinderToRight()
	{
		// Arrange
		const int successValue = 42;
		var either = Either<string, int>.FromRight(successValue);

        static Task<Either<string, string>> AsyncBinder(int val) =>
			Task.FromResult(Either<string, string>.FromRight($"Value: {val}"));

		// Act
		var result = await either.BindAsync(AsyncBinder);

		// Assert
		result.IsRight.Should().BeTrue();
		result.Right.Should().Be($"Value: {successValue}");
	}

	[Fact]
	public async Task BindAsync_WhenContainsLeftValue_ShouldNotApplyBinder()
	{
		// Arrange
		const string errorMessage = "Error occurred";
		var either = Either<string, int>.FromLeft(errorMessage);

        static Task<Either<string, string>> AsyncBinder(int val) =>
			Task.FromResult(Either<string, string>.FromRight($"Value: {val}"));

		// Act
		var result = await either.BindAsync(AsyncBinder);

		// Assert
		result.IsLeft.Should().BeTrue();
		result.Left.Should().Be(errorMessage);
	}

	[Fact]
	public async Task BindAsync_WhenBinderReturnsLeft_ShouldReturnLeftValue()
	{
		// Arrange
		const int successValue = 42;
		var either = Either<string, int>.FromRight(successValue);
		const string errorMessage = "Validation failed";

        static Task<Either<string, string>> FailingBinder(int _) =>
			Task.FromResult(Either<string, string>.FromLeft(errorMessage));

		// Act
		var result = await either.BindAsync(FailingBinder);

		// Assert
		result.IsLeft.Should().BeTrue();
		result.Left.Should().Be(errorMessage);
	}

	[Fact]
	public async Task BindAsync_WithAsyncBinderThatThrows_ShouldPropagateException()
	{
		// Arrange
		const int successValue = 42;
		var either = Either<string, int>.FromRight(successValue);

        static Task<Either<string, string>> ThrowingBinder(int _) =>
			throw new InvalidOperationException("Binder exception");

		// Act & Assert
		await either.Invoking(e => e.BindAsync(ThrowingBinder))
			.Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("Binder exception");
	}

	#endregion

	#region MatchAsync Method

	[Fact]
	public async Task MatchAsync_WhenContainsLeftValue_ShouldInvokeLeftAsyncFunction()
	{
		// Arrange
		const string errorMessage = "Error occurred";
		var either = Either<string, int>.FromLeft(errorMessage);
		
		Task<string> LeftFunc(string msg) => Task.FromResult($"Error: {msg}");
		Task<string> RightFunc(int val) => Task.FromResult($"Success: {val}");

		// Act
		var result = await either.MatchAsync(LeftFunc, RightFunc);

		// Assert
		result.Should().Be($"Error: {errorMessage}");
	}

	[Fact]
	public async Task MatchAsync_WhenContainsRightValue_ShouldInvokeRightAsyncFunction()
	{
		// Arrange
		const int successValue = 42;
		var either = Either<string, int>.FromRight(successValue);
		
		Task<string> LeftFunc(string msg) => Task.FromResult($"Error: {msg}");
		Task<string> RightFunc(int val) => Task.FromResult($"Success: {val}");

		// Act
		var result = await either.MatchAsync(LeftFunc, RightFunc);

		// Assert
		result.Should().Be($"Success: {successValue}");
	}

	[Fact]
	public async Task MatchAsync_WhenLeftFunctionThrows_ShouldPropagateException()
	{
		// Arrange
		const string errorMessage = "Error occurred";
		var either = Either<string, int>.FromLeft(errorMessage);
		
		Task<string> ThrowingLeftFunc(string _) => 
			throw new InvalidOperationException("Left function exception");
		Task<string> RightFunc(int val) => Task.FromResult($"Success: {val}");

		// Act & Assert
		await either.Invoking(e => e.MatchAsync(ThrowingLeftFunc, RightFunc))
			.Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("Left function exception");
	}

	[Fact]
	public async Task MatchAsync_WhenRightFunctionThrows_ShouldPropagateException()
	{
		// Arrange
		const int successValue = 42;
		var either = Either<string, int>.FromRight(successValue);
		
		Task<string> LeftFunc(string msg) => Task.FromResult($"Error: {msg}");
		Task<string> ThrowingRightFunc(int _) => 
			throw new InvalidOperationException("Right function exception");

		// Act & Assert
		await either.Invoking(e => e.MatchAsync(LeftFunc, ThrowingRightFunc))
			.Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("Right function exception");
	}

	#endregion

	#region TryAsync Method

	[Fact]
	public async Task TryAsync_WhenAsyncFunctionSucceeds_ShouldReturnRightWithResult()
	{
		// Arrange
		const string expectedResult = "Success";

        static Task<string> SuccessFunc() => Task.FromResult(expectedResult);

		// Act
		var result = await EitherAsyncExtensions.TryAsync(SuccessFunc);

		// Assert
		result.IsRight.Should().BeTrue();
		result.Right.Should().Be(expectedResult);
	}

	[Fact]
	public async Task TryAsync_WhenAsyncFunctionThrows_ShouldReturnLeftWithException()
	{
		// Arrange
		var expectedException = new InvalidOperationException("Test exception");
		
		Task<string> FailingFunc() => 
			throw expectedException;

		// Act
		var result = await EitherAsyncExtensions.TryAsync(FailingFunc);

		// Assert
		result.IsLeft.Should().BeTrue();
		result.Left.Should().BeOfType<InvalidOperationException>();
		result.Left.Message.Should().Be("Test exception");
	}

	[Fact]
	public async Task TryAsync_WhenSynchronousExceptionThrown_ShouldCaptureInLeft()
	{
        // Arrange
        static Task<string> SyncFailingFunc()
		{
			throw new ArgumentException("Sync exception");
		}

		// Act
		var result = await EitherAsyncExtensions.TryAsync(SyncFailingFunc);

		// Assert
		result.IsLeft.Should().BeTrue();
		result.Left.Should().BeOfType<ArgumentException>();
		result.Left.Message.Should().Be("Sync exception");
	}

	[Fact]
	public async Task TryAsync_WithComplexAsyncOperation_ShouldHandleSuccessCorrectly()
	{
        // Arrange
        static async Task<int> ComplexOperation()
		{
			await Task.Delay(10); // Simulate async work
			return 42;
		}

		// Act
		var result = await EitherAsyncExtensions.TryAsync(ComplexOperation);

		// Assert
		result.IsRight.Should().BeTrue();
		result.Right.Should().Be(42);
	}

	[Fact]
	public async Task TryAsync_WithComplexFailingOperation_ShouldHandleExceptionCorrectly()
	{
        // Arrange
        static async Task<int> ComplexFailingOperation()
		{
			await Task.Delay(10); // Simulate async work
			throw new InvalidOperationException("Complex operation failed");
		}

		// Act
		var result = await EitherAsyncExtensions.TryAsync(ComplexFailingOperation);

		// Assert
		result.IsLeft.Should().BeTrue();
		result.Left.Should().BeOfType<InvalidOperationException>();
		result.Left.Message.Should().Be("Complex operation failed");
	}

	#endregion

	#region Complex Chaining Scenarios

	[Fact]
	public async Task ChainedAsyncOperations_ShouldWorkCorrectly()
	{
		// Arrange
		const int initialValue = 10;
		var either = Either<string, int>.FromRight(initialValue);

		// Define local functions for the operations
		Task<int> MultiplyByTwo(int val) => Task.FromResult(val * 2);
		
		Task<Either<string, int>> ConditionalAdd(int val) => 
			Task.FromResult(val > 15 
				? Either<string, int>.FromRight(val + 5) // 25
				: Either<string, int>.FromLeft("Value too small"));
				
		Task<string> ToString(int val) => Task.FromResult(val.ToString());

		// Act
		var step1 = await either.MapRightAsync(MultiplyByTwo); // 20
		var step2 = await step1.BindAsync(ConditionalAdd); // 25
		var result = await step2.MapRightAsync(ToString); // "25"

		// Assert
		result.IsRight.Should().BeTrue();
		result.Right.Should().Be("25");
	}

	[Fact]
	public async Task ChainedAsyncOperations_WhenIntermediateOperationFails_ShouldShortCircuit()
	{
		// Arrange
		const int initialValue = 5;
		var either = Either<string, int>.FromRight(initialValue);

		// Define local functions for the operations
		Task<int> MultiplyByTwo(int val) => Task.FromResult(val * 2);
		
		Task<Either<string, int>> ConditionalAdd(int val) => 
			Task.FromResult(val > 15 
				? Either<string, int>.FromRight(val + 5)
				: Either<string, int>.FromLeft("Value too small"));
				
		Task<string> ToString(int val) => Task.FromResult(val.ToString());

		// Act
		var step1 = await either.MapRightAsync(MultiplyByTwo); // 10
		var step2 = await step1.BindAsync(ConditionalAdd); // Will be Left
		var result = await step2.MapRightAsync(ToString);

		// Assert
		result.IsLeft.Should().BeTrue();
		result.Left.Should().Be("Value too small");
	}

	[Fact]
	public async Task MixingAsyncAndSyncOperations_ShouldWorkCorrectly()
	{
		// Arrange
		const int initialValue = 10;
		var either = Either<string, int>.FromRight(initialValue);

		// Define local functions for the operations
		int MultiplyByTwo(int val) => val * 2;
		Task<int> AddFive(int val) => Task.FromResult(val + 5);
		
		Either<string, string> FormatValue(int val) => 
			val >= 25 
				? Either<string, string>.FromRight($"Value: {val}")
				: Either<string, string>.FromLeft("Value too small");
				
		Task<string> ToUpperCase(string val) => Task.FromResult(val.ToUpperInvariant());

		// Act
		var step1 = either.MapRight(MultiplyByTwo); // Sync - 20
		var step2 = await step1.MapRightAsync(AddFive); // Async - 25
		var step3 = step2.Bind(FormatValue); // Sync - "Value: 25"
		var result = await step3.MapRightAsync(ToUpperCase); // Async - "VALUE: 25"

		// Assert
		result.IsRight.Should().BeTrue();
		result.Right.Should().Be("VALUE: 25");
	}

	[Fact]
	public async Task CombiningTryAsyncWithOtherExtensions_ShouldWorkCorrectly()
	{
		// Arrange
		async Task<int> FetchValueAsync()
		{
			await Task.Delay(10); // Simulate fetching data
			return 42;
		}

		// Define local functions for operations
		int MultiplyByTwo(int val) => val * 2;
		string FormatError(Exception ex) => $"Error: {ex.Message}";
		
		Either<string, string> CheckValueAndFormat(int val) => 
			val > 80 
				? Either<string, string>.FromRight($"Success: {val}")
				: Either<string, string>.FromLeft("Value too small");

		// Act
		var fetchResult = await EitherAsyncExtensions.TryAsync(FetchValueAsync);
		var multiplied = fetchResult.MapRight(MultiplyByTwo); // 84
		var withFormattedError = multiplied.MapLeft(FormatError);
		var result = withFormattedError.Bind(CheckValueAndFormat);

		// Assert
		result.IsRight.Should().BeTrue();
		result.Right.Should().Be("Success: 84");
	}

	[Fact]
	public async Task TryAsync_IntegratedWithMatchAsync_ShouldWorkCorrectly()
	{
		// Arrange
		Task<int> SuccessFunc() => Task.FromResult(42);
		
		Task<string> FormatError(Exception ex) => Task.FromResult($"Error: {ex.Message}");
		Task<string> FormatSuccess(int val) => Task.FromResult($"Success: {val}");

		// Act
		var result = await EitherAsyncExtensions.TryAsync(SuccessFunc);
		var matchResult = await result.MatchAsync(FormatError, FormatSuccess);

		// Assert
		matchResult.Should().Be("Success: 42");
	}

	#endregion
}