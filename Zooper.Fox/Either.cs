// ReSharper disable MemberCanBePrivate.Global

using System;

namespace Zooper.Fox;

/// <summary>
/// Represents a disjoint union of two types, where an instance can hold a value of either the left or the right type.
/// This is a fundamental functional programming concept used for representing operations that can succeed or fail,
/// or for any scenario where a value can be one of two distinct types.
/// 
/// <para>
/// By convention, in error handling scenarios:
/// - The Left value typically represents an error or failure case
/// - The Right value typically represents a success or valid result
/// </para>
/// 
/// <example>
/// <code>
/// // Create an Either representing a successful operation
/// Either&lt;string, int&gt; success = Either&lt;string, int&gt;.FromRight(42);
/// 
/// // Create an Either representing a failed operation
/// Either&lt;string, int&gt; failure = Either&lt;string, int&gt;.FromLeft("Operation failed");
/// 
/// // Use pattern matching to handle both cases
/// string result = success.Match(
///     left => $"Error: {left}",
///     right => $"Result: {right}"
/// );
/// </code>
/// </example>
/// </summary>
/// <typeparam name="TLeft">The type of the Left value, typically representing an error or failure case.</typeparam>
/// <typeparam name="TRight">The type of the Right value, typically representing a success or valid result.</typeparam>
public class Either<TLeft, TRight>
{
	private readonly TLeft? _left;
	private readonly TRight? _right;
	private readonly EitherState _state;

	/// <summary>
	/// Gets the Left value of this Either instance.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Thrown when attempting to access the Left value when this instance contains a Right value.
	/// Always check <see cref="IsLeft"/> before accessing this property.
	/// </exception>
	/// <value>The Left value if present; otherwise throws an exception.</value>
	public TLeft Left => IsLeft
		? _left!
		: throw new InvalidOperationException("Cannot access Left when IsLeft is false.");

	/// <summary>
	/// Gets the Right value of this Either instance.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Thrown when attempting to access the Right value when this instance contains a Left value.
	/// Always check <see cref="IsRight"/> before accessing this property.
	/// </exception>
	/// <value>The Right value if present; otherwise throws an exception.</value>
	public TRight Right => IsRight
		? _right!
		: throw new InvalidOperationException("Cannot access Right when IsRight is false.");

	/// <summary>
	/// Indicates whether this instance holds a Left value.
	/// Use this property to safely check before accessing the <see cref="Left"/> property.
	/// </summary>
	/// <value>true if this instance contains a Left value; otherwise, false.</value>
	public bool IsLeft => _state == EitherState.Left;

	/// <summary>
	/// Indicates whether this instance holds a Right value.
	/// Use this property to safely check before accessing the <see cref="Right"/> property.
	/// </summary>
	/// <value>true if this instance contains a Right value; otherwise, false.</value>
	public bool IsRight => _state == EitherState.Right;

	/// <summary>
	/// Initializes a new instance of the <see cref="Either{TLeft, TRight}"/> class with a Left value.
	/// This constructor is protected to encourage using the factory methods <see cref="FromLeft"/>
	/// and <see cref="FromRight"/> to create instances.
	/// </summary>
	/// <param name="left">The Left value to initialize.</param>
	protected Either(TLeft left)
	{
		_left = left;
		_right = default;
		_state = EitherState.Left;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Either{TLeft, TRight}"/> class with a Right value.
	/// This constructor is protected to encourage using the factory methods <see cref="FromLeft"/>
	/// and <see cref="FromRight"/> to create instances.
	/// </summary>
	/// <param name="right">The Right value to initialize.</param>
	protected Either(TRight right)
	{
		_right = right;
		_left = default;
		_state = EitherState.Right;
	}

	/// <summary>
	/// Creates a new <see cref="Either{TLeft, TRight}"/> instance from a Left value.
	/// In error handling scenarios, this typically represents a failure result.
	/// </summary>
	/// <param name="left">The Left value to wrap.</param>
	/// <returns>An <see cref="Either{TLeft, TRight}"/> instance containing the Left value.</returns>
	/// <example>
	/// <code>
	/// // Create an Either representing a validation error
	/// var error = Either&lt;string, User&gt;.FromLeft("Invalid username");
	/// </code>
	/// </example>
	public static Either<TLeft, TRight> FromLeft(TLeft left) => new(left);

	/// <summary>
	/// Creates a new <see cref="Either{TLeft, TRight}"/> instance from a Right value.
	/// In error handling scenarios, this typically represents a successful result.
	/// </summary>
	/// <param name="right">The Right value to wrap.</param>
	/// <returns>An <see cref="Either{TLeft, TRight}"/> instance containing the Right value.</returns>
	/// <example>
	/// <code>
	/// // Create an Either representing a successful user lookup
	/// var success = Either&lt;string, User&gt;.FromRight(user);
	/// </code>
	/// </example>
	public static Either<TLeft, TRight> FromRight(TRight right) => new(right);

	/// <summary>
	/// Matches the current value to one of two possible functions and returns the result of the matched function.
	/// This is the primary way to safely extract and transform the value from an Either instance.
	/// </summary>
	/// <typeparam name="T">The return type of the matching functions.</typeparam>
	/// <param name="leftFunc">The function to invoke if the instance holds a Left value.</param>
	/// <param name="rightFunc">The function to invoke if the instance holds a Right value.</param>
	/// <returns>The result of the invoked function.</returns>
	/// <exception cref="InvalidOperationException">Thrown if both Left and Right values are null.</exception>
	/// <example>
	/// <code>
	/// // Process a validation result
	/// string message = validationResult.Match(
	///     error => $"Validation failed: {error}",
	///     user => $"Welcome, {user.Name}!"
	/// );
	/// </code>
	/// </example>
	public T Match<T>(
		Func<TLeft, T> leftFunc,
		Func<TRight, T> rightFunc)
	{
		if (IsLeft)
		{
			return leftFunc(_left!);
		}

		if (IsRight)
		{
			return rightFunc(_right!);
		}

		throw new InvalidOperationException("Invalid Either state.");
	}

	/// <summary>
	/// Matches the current value to one of two possible actions and invokes the matched action.
	/// This is useful when you want to perform side effects based on the state of the Either
	/// but don't need to return a value.
	/// </summary>
	/// <param name="leftAction">The action to invoke if the instance holds a Left value.</param>
	/// <param name="rightAction">The action to invoke if the instance holds a Right value.</param>
	/// <exception cref="InvalidOperationException">Thrown if both Left and Right values are null.</exception>
	/// <example>
	/// <code>
	/// // Perform different actions based on result
	/// result.Match(
	///     error => LogError(error),
	///     user => SendWelcomeEmail(user)
	/// );
	/// </code>
	/// </example>
	public void Match(
		Action<TLeft> leftAction,
		Action<TRight> rightAction)
	{
		if (IsLeft)
		{
			leftAction(_left!);
			return;
		}

		if (IsRight)
		{
			rightAction(_right!);
			return;
		}

		throw new InvalidOperationException("Invalid Either state.");
	}

	/// <summary>
	/// Implicitly converts a <typeparamref name="TLeft"/> value to an <see cref="Either{TLeft, TRight}"/> instance.
	/// This allows for more concise code when creating Left instances.
	/// </summary>
	/// <param name="left">The Left value to convert.</param>
	/// <returns>An <see cref="Either{TLeft, TRight}"/> instance containing the Left value.</returns>
	/// <example>
	/// <code>
	/// // Using implicit conversion to create a Left instance
	/// Either&lt;string, int&gt; error = "Invalid input";
	/// </code>
	/// </example>
	public static implicit operator Either<TLeft, TRight>(TLeft left) => FromLeft(left);

	/// <summary>
	/// Implicitly converts a <typeparamref name="TRight"/> value to an <see cref="Either{TLeft, TRight}"/> instance.
	/// This allows for more concise code when creating Right instances.
	/// </summary>
	/// <param name="right">The Right value to convert.</param>
	/// <returns>An <see cref="Either{TLeft, TRight}"/> instance containing the Right value.</returns>
	/// <example>
	/// <code>
	/// // Using implicit conversion to create a Right instance
	/// Either&lt;string, int&gt; success = 42;
	/// </code>
	/// </example>
	public static implicit operator Either<TLeft, TRight>(TRight right) => FromRight(right);
	
	/// <summary>
	/// Represents the internal state of an Either instance.
	/// </summary>
	private enum EitherState
	{
		/// <summary>
		/// The instance contains a Left value.
		/// </summary>
		Left,
		
		/// <summary>
		/// The instance contains a Right value.
		/// </summary>
		Right
	}
}