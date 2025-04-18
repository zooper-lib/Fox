using System;
using System.Collections.Generic;

namespace Zooper.Fox;

/// <summary>
/// Provides LINQ-style extension methods for working with <see cref="Either{TLeft,TRight}"/> instances.
/// <para>
/// These extensions allow Either instances to be used in LINQ query expressions and to follow
/// functional programming patterns familiar to LINQ users. They make it possible to compose
/// and transform Either values in a fluent, declarative way.
/// </para>
/// </summary>
public static class EitherLinqExtensions
{
	/// <summary>
	/// Projects the Right value of an Either into a new form using a selector function.
	/// This is the implementation of the LINQ Select operation for Either types.
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the source Right value.</typeparam>
	/// <typeparam name="TResult">The type of the target Right value.</typeparam>
	/// <param name="either">The Either instance to transform.</param>
	/// <param name="selector">A transform function to apply to the Right value.</param>
	/// <returns>
	/// A new Either instance with either the original Left value or the transformed Right value.
	/// </returns>
	/// <example>
	/// <code>
	/// // Use Select to transform a successful result
	/// var userNames = users
	///     .Select(user => user.Name)
	///     .ToList();
	/// </code>
	/// </example>
	public static Either<TLeft, TResult> Select<TLeft, TRight, TResult>(
		this Either<TLeft, TRight> either,
		Func<TRight, TResult> selector) =>
		either.Match(
			Either<TLeft, TResult>.FromLeft,
			right => Either<TLeft, TResult>.FromRight(selector(right))
		);

	/// <summary>
	/// Filters an Either based on a predicate applied to the Right value.
	/// This is the implementation of the LINQ Where operation for Either types.
	/// <para>
	/// Note: If the predicate returns false and the Either contains a Right value,
	/// this method throws an exception. This behavior is different from typical
	/// LINQ Where operations because Either doesn't naturally represent "no value"
	/// in the same way that collections do.
	/// </para>
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the Right value.</typeparam>
	/// <param name="either">The Either instance to filter.</param>
	/// <param name="predicate">A function to test the Right value for a condition.</param>
	/// <returns>The original Either if it contains a Left value or if the predicate returns true for the Right value.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the Either contains a Right value and the predicate returns false.</exception>
	/// <example>
	/// <code>
	/// // Filter out negative numbers
	/// // Note: This will throw if the result contains a negative number
	/// var positiveNumber = numberResult.Where(n => n > 0);
	/// </code>
	/// </example>
	public static Either<TLeft, TRight> Where<TLeft, TRight>(
		this Either<TLeft, TRight> either,
		Func<TRight, bool> predicate) =>
		either.Match(
			_ => either,
			right => predicate(right)
				? either
				: throw new InvalidOperationException("Predicate returned false for Right value")
		);

	/// <summary>
	/// Projects the Right value of an Either into a new Either, and then flattens the resulting Either into a single
	/// Either. This is the implementation of the LINQ SelectMany operation, enabling LINQ query syntax with Either types.
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the source Right value.</typeparam>
	/// <typeparam name="TIntermediate">The type of the intermediate Right value.</typeparam>
	/// <typeparam name="TResult">The type of the resulting Right value.</typeparam>
	/// <param name="either">The source Either instance.</param>
	/// <param name="selector">A transform function to apply to the Right value, which returns a new Either.</param>
	/// <param name="resultSelector">A function that combines the original Right value and the intermediate Right value.</param>
	/// <returns>
	/// A new Either instance resulting from applying the selector function and flattening.
	/// </returns>
	/// <example>
	/// <code>
	/// // Use LINQ query syntax with Either types
	/// var result = from user in GetUser(id)
	///              from orders in GetOrders(user.Id)
	///              select new { User = user, Orders = orders };
	/// </code>
	/// </example>
	public static Either<TLeft, TResult> SelectMany<TLeft, TRight, TIntermediate, TResult>(
		this Either<TLeft, TRight> either,
		Func<TRight, Either<TLeft, TIntermediate>> selector,
		Func<TRight, TIntermediate, TResult> resultSelector) =>
		either.Match(
			Either<TLeft, TResult>.FromLeft,
			right => selector(right).Match(
				Either<TLeft, TResult>.FromLeft,
				rightInner => Either<TLeft, TResult>.FromRight(resultSelector(right, rightInner))
			)
		);

	/// <summary>
	/// Returns the original Either if it contains a Right value; otherwise, returns a new Either
	/// containing the specified default Right value.
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the Right value.</typeparam>
	/// <param name="either">The Either instance to check.</param>
	/// <param name="rightValue">The default Right value to use if the Either contains a Left value.</param>
	/// <returns>
	/// The original Either if it contains a Right value; otherwise, a new Either containing the default Right value.
	/// </returns>
	/// <example>
	/// <code>
	/// // Provide a default value if the result is an error
	/// var userOrDefault = userResult.OrElse(new User { Name = "Guest" });
	/// </code>
	/// </example>
	public static Either<TLeft, TRight> OrElse<TLeft, TRight>(
		this Either<TLeft, TRight> either,
		TRight rightValue) =>
		either.IsRight
			? either
			: Either<TLeft, TRight>.FromRight(rightValue);

	/// <summary>
	/// Returns the original Either if it contains a Right value; otherwise, returns a new Either
	/// containing a Right value created by applying the specified function to the Left value.
	/// <para>
	/// This is useful for recovering from errors by transforming an error into a valid result.
	/// </para>
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the Right value.</typeparam>
	/// <param name="either">The Either instance to check.</param>
	/// <param name="rightFactory">A function that transforms the Left value into a Right value.</param>
	/// <returns>
	/// The original Either if it contains a Right value; otherwise, a new Either containing
	/// a Right value created by applying the specified function to the Left value.
	/// </returns>
	/// <example>
	/// <code>
	/// // Transform an error into a valid result
	/// var userOrDefault = userResult.OrElse(error => 
	///     new User { Name = "Guest", Message = $"Error: {error}" });
	/// </code>
	/// </example>
	public static Either<TLeft, TRight> OrElse<TLeft, TRight>(
		this Either<TLeft, TRight> either,
		Func<TLeft, TRight> rightFactory) =>
		either.IsRight
			? either
			: Either<TLeft, TRight>.FromRight(rightFactory(either.Left));

	/// <summary>
	/// Combines two Either instances into a single Either containing a tuple of their Right values.
	/// If either of the input Eithers contains a Left value, the result will contain that Left value.
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight1">The type of the Right value in the first Either.</typeparam>
	/// <typeparam name="TRight2">The type of the Right value in the second Either.</typeparam>
	/// <param name="first">The first Either instance to combine.</param>
	/// <param name="second">The second Either instance to combine.</param>
	/// <returns>
	/// A new Either containing either a Left value from one of the inputs or a tuple of both Right values.
	/// </returns>
	/// <example>
	/// <code>
	/// // Combine user and profile data, but propagate any errors
	/// var combined = userResult.Zip(profileResult);
	/// 
	/// // Process the combined data
	/// combined.Match(
	///     error => Console.WriteLine($"Error: {error}"),
	///     tuple => {
	///         var (user, profile) = tuple;
	///         Console.WriteLine($"User: {user.Name}, Profile: {profile.Bio}");
	///     }
	/// );
	/// </code>
	/// </example>
	public static Either<TLeft, (TRight1, TRight2)> Zip<TLeft, TRight1, TRight2>(
		this Either<TLeft, TRight1> first,
		Either<TLeft, TRight2> second) =>
		first.Match(
			Either<TLeft, (TRight1, TRight2)>.FromLeft,
			right1 => second.Match(
				Either<TLeft, (TRight1, TRight2)>.FromLeft,
				right2 => Either<TLeft, (TRight1, TRight2)>.FromRight((right1, right2))
			)
		);

	/// <summary>
	/// Flattens a nested Either instance.
	/// <para>
	/// When you have an Either of an Either, this method collapses them into a single Either.
	/// </para>
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the innermost Right value.</typeparam>
	/// <param name="either">The nested Either instance to flatten.</param>
	/// <returns>A flattened Either instance.</returns>
	/// <example>
	/// <code>
	/// // A nested Either might result from certain operations
	/// Either&lt;string, Either&lt;string, int&gt;&gt; nested = // ...
	/// 
	/// // Flatten it to a single Either
	/// Either&lt;string, int&gt; flattened = nested.Flatten();
	/// </code>
	/// </example>
	public static Either<TLeft, TRight> Flatten<TLeft, TRight>(this Either<TLeft, Either<TLeft, TRight>> either) =>
		either.Match(
			Either<TLeft, TRight>.FromLeft,
			innerEither => innerEither
		);

	/// <summary>
	/// Converts the Left value of an Either to an enumerable with zero or one elements.
	/// <para>
	/// This method returns an enumerable containing the Left value if the Either contains a Left value;
	/// otherwise, it returns an empty enumerable.
	/// </para>
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the Right value.</typeparam>
	/// <param name="either">The Either instance to convert.</param>
	/// <returns>An enumerable containing zero or one elements, depending on whether the Either contains a Left value.</returns>
	/// <example>
	/// <code>
	/// // Convert error results to an enumerable and process them
	/// var errors = results
	///     .SelectMany(r => r.LeftToEnumerable())
	///     .ToList();
	///     
	/// if (errors.Any())
	/// {
	///     Console.WriteLine($"Found {errors.Count} errors:");
	///     foreach (var error in errors)
	///     {
	///         Console.WriteLine($"- {error}");
	///     }
	/// }
	/// </code>
	/// </example>
	public static IEnumerable<TLeft> LeftToEnumerable<TLeft, TRight>(this Either<TLeft, TRight> either)
	{
		if (either.IsLeft)
		{
			yield return either.Left;
		}
	}

	/// <summary>
	/// Converts the Right value of an Either to an enumerable with zero or one elements.
	/// <para>
	/// This method returns an enumerable containing the Right value if the Either contains a Right value;
	/// otherwise, it returns an empty enumerable.
	/// </para>
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the Right value.</typeparam>
	/// <param name="either">The Either instance to convert.</param>
	/// <returns>An enumerable containing zero or one elements, depending on whether the Either contains a Right value.</returns>
	/// <example>
	/// <code>
	/// // Convert successful results to an enumerable and process them
	/// var successfulUsers = userResults
	///     .SelectMany(r => r.RightToEnumerable())
	///     .ToList();
	///     
	/// foreach (var user in successfulUsers)
	/// {
	///     Console.WriteLine($"Processing user: {user.Name}");
	/// }
	/// </code>
	/// </example>
	public static IEnumerable<TRight> RightToEnumerable<TLeft, TRight>(this Either<TLeft, TRight> either)
	{
		if (either.IsRight)
		{
			yield return either.Right;
		}
	}
}