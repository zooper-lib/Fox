using System;

namespace Zooper.Fox;

/// <summary>
/// Provides extension methods for working with <see cref="Either{TLeft,TRight}"/> instances.
/// <para>
/// These extension methods make it easier to transform and combine Either values in a functional style,
/// allowing for more concise and readable code when working with operations that might fail.
/// </para>
/// </summary>
public static class EitherExtensions
{
	/// <summary>
	/// Attempts to get the Right value from an Either instance, returning a boolean to indicate success.
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the Right value.</typeparam>
	/// <param name="either">The Either instance to extract the Right value from.</param>
	/// <param name="right">When this method returns, contains the Right value if the instance contains one; otherwise, the default value for the type.</param>
	/// <returns>true if the Either instance contains a Right value; otherwise, false.</returns>
	/// <example>
	/// <code>
	/// var result = GetUserById(id);
	/// if (result.TryGetRight(out var user))
	/// {
	///     Console.WriteLine($"User found: {user.Name}");
	/// }
	/// else
	/// {
	///     Console.WriteLine("User not found");
	/// }
	/// </code>
	/// </example>
	public static bool TryGetRight<TLeft, TRight>(
		this Either<TLeft, TRight> either,
		out TRight? right)
	{
		right = either.IsRight ? either.Right : default;
		return either.IsRight;
	}

	/// <summary>
	/// Attempts to get the Left value from an Either instance, returning a boolean to indicate success.
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the Right value.</typeparam>
	/// <param name="either">The Either instance to extract the Left value from.</param>
	/// <param name="left">When this method returns, contains the Left value if the instance contains one; otherwise, the default value for the type.</param>
	/// <returns>true if the Either instance contains a Left value; otherwise, false.</returns>
	/// <example>
	/// <code>
	/// var result = ValidateUser(user);
	/// if (result.TryGetLeft(out var error))
	/// {
	///     Console.WriteLine($"Validation failed: {error}");
	/// }
	/// else
	/// {
	///     Console.WriteLine("User is valid");
	/// }
	/// </code>
	/// </example>
	public static bool TryGetLeft<TLeft, TRight>(
		this Either<TLeft, TRight> either,
		out TLeft? left)
	{
		left = either.IsLeft ? either.Left : default;
		return either.IsLeft;
	}

	/// <summary>
	/// Transforms the Left value of an Either instance using the provided mapping function.
	/// <para>
	/// If the instance contains a Left value, applies the mapper to it and returns a new
	/// Either with the resulting value as Left. If the instance contains a Right value,
	/// returns a new Either with the same Right value.
	/// </para>
	/// </summary>
	/// <typeparam name="TLeft">The type of the source Left value.</typeparam>
	/// <typeparam name="TRight">The type of the Right value.</typeparam>
	/// <typeparam name="TNewLeft">The type of the new Left value.</typeparam>
	/// <param name="either">The Either instance to transform.</param>
	/// <param name="mapper">The function to apply to the Left value.</param>
	/// <returns>A new Either instance with either the transformed Left value or the original Right value.</returns>
	/// <example>
	/// <code>
	/// // Convert error messages to localized versions
	/// var result = GetUser(id)
	///     .MapLeft(errorCode => TranslateErrorCode(errorCode, currentLanguage));
	/// </code>
	/// </example>
	public static Either<TNewLeft, TRight> MapLeft<TLeft, TRight, TNewLeft>(
		this Either<TLeft, TRight> either,
		Func<TLeft, TNewLeft> mapper) =>
		either.Match(
			left => Either<TNewLeft, TRight>.FromLeft(mapper(left)),
			Either<TNewLeft, TRight>.FromRight
		);

	/// <summary>
	/// Transforms the Right value of an Either instance using the provided mapping function.
	/// <para>
	/// If the instance contains a Right value, applies the mapper to it and returns a new
	/// Either with the resulting value as Right. If the instance contains a Left value,
	/// returns a new Either with the same Left value.
	/// </para>
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the source Right value.</typeparam>
	/// <typeparam name="TNewRight">The type of the new Right value.</typeparam>
	/// <param name="either">The Either instance to transform.</param>
	/// <param name="mapper">The function to apply to the Right value.</param>
	/// <returns>A new Either instance with either the original Left value or the transformed Right value.</returns>
	/// <example>
	/// <code>
	/// // Extract just the name from a user object
	/// Either&lt;string, string&gt; userName = GetUser(id)
	///     .MapRight(user => user.Name);
	/// </code>
	/// </example>
	public static Either<TLeft, TNewRight> MapRight<TLeft, TRight, TNewRight>(
		this Either<TLeft, TRight> either,
		Func<TRight, TNewRight> mapper) =>
		either.Match(
			Either<TLeft, TNewRight>.FromLeft,
			right => Either<TLeft, TNewRight>.FromRight(mapper(right))
		);

	/// <summary>
	/// Binds the Right value of an Either instance to a function that returns a new Either,
	/// effectively chaining multiple operations that might fail.
	/// <para>
	/// This method is particularly useful for composing multiple operations where each operation
	/// returns an Either type, representing a computation that might fail.
	/// </para>
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the source Right value.</typeparam>
	/// <typeparam name="TNewRight">The type of the new Right value.</typeparam>
	/// <param name="either">The Either instance to bind.</param>
	/// <param name="binder">The function that transforms the Right value into a new Either.</param>
	/// <returns>
	/// The result of applying the binder function to the Right value if the instance contains a Right value;
	/// otherwise, a new Either containing the original Left value.
	/// </returns>
	/// <example>
	/// <code>
	/// // Chain operations that might fail
	/// var result = GetUser(id)
	///     .Bind(user => GetUserOrders(user.Id))
	///     .Bind(orders => ProcessOrders(orders));
	/// </code>
	/// </example>
	public static Either<TLeft, TNewRight> Bind<TLeft, TRight, TNewRight>(
		this Either<TLeft, TRight> either,
		Func<TRight, Either<TLeft, TNewRight>> binder) =>
		either.Match(
			Either<TLeft, TNewRight>.FromLeft,
			binder
		);
}