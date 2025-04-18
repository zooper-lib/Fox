using System;
using System.Threading.Tasks;

namespace Zooper.Fox;

/// <summary>
/// Provides asynchronous extension methods for working with <see cref="Either{TLeft,TRight}"/> instances.
/// <para>
/// These extension methods make it easier to work with asynchronous operations that produce Either values,
/// allowing for composing asynchronous operations that might fail in a functional and declarative way.
/// </para>
/// </summary>
public static class EitherAsyncExtensions
{
	/// <summary>
	/// Asynchronously matches the value of an Either instance to one of two possible async functions and returns the result.
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the Right value.</typeparam>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	/// <param name="either">The Either instance to match.</param>
	/// <param name="leftFunc">The async function to invoke if the instance contains a Left value.</param>
	/// <param name="rightFunc">The async function to invoke if the instance contains a Right value.</param>
	/// <returns>A task representing the asynchronous operation, with the result of the appropriate function.</returns>
	/// <example>
	/// <code>
	/// var result = await userResult.MatchAsync(
	///     async error => await LogErrorAsync(error),
	///     async user => await GenerateProfileAsync(user)
	/// );
	/// </code>
	/// </example>
	public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(
		this Either<TLeft, TRight> either,
		Func<TLeft, Task<TResult>> leftFunc,
		Func<TRight, Task<TResult>> rightFunc) =>
		either.IsLeft
			? await leftFunc(either.Left!)
			: await rightFunc(either.Right!);

	/// <summary>
	/// Asynchronously transforms the Left value of an Either instance using the provided async mapping function.
	/// </summary>
	/// <typeparam name="TLeft">The type of the source Left value.</typeparam>
	/// <typeparam name="TRight">The type of the Right value.</typeparam>
	/// <typeparam name="TNewLeft">The type of the new Left value after mapping.</typeparam>
	/// <param name="either">The Either instance to transform.</param>
	/// <param name="mapper">The async function to apply to the Left value.</param>
	/// <returns>
	/// A task representing the asynchronous operation, with a new Either containing either 
	/// the transformed Left value or the original Right value.
	/// </returns>
	/// <example>
	/// <code>
	/// // Asynchronously translate error codes to detailed error messages
	/// var result = await validationResult.MapLeftAsync(
	///     async code => await FetchErrorDetailsAsync(code)
	/// );
	/// </code>
	/// </example>
	public static async Task<Either<TNewLeft, TRight>> MapLeftAsync<TLeft, TRight, TNewLeft>(
		this Either<TLeft, TRight> either,
		Func<TLeft, Task<TNewLeft>> mapper) =>
		either.IsLeft
			? Either<TNewLeft, TRight>.FromLeft(await mapper(either.Left))
			: Either<TNewLeft, TRight>.FromRight(either.Right);

	/// <summary>
	/// Asynchronously transforms the Right value of an Either instance using the provided async mapping function.
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the source Right value.</typeparam>
	/// <typeparam name="TNewRight">The type of the new Right value after mapping.</typeparam>
	/// <param name="either">The Either instance to transform.</param>
	/// <param name="mapper">The async function to apply to the Right value.</param>
	/// <returns>
	/// A task representing the asynchronous operation, with a new Either containing either
	/// the original Left value or the transformed Right value.
	/// </returns>
	/// <example>
	/// <code>
	/// // Asynchronously enrich a user with additional profile data
	/// var result = await userResult.MapRightAsync(
	///     async user => await FetchUserProfileAsync(user)
	/// );
	/// </code>
	/// </example>
	public static async Task<Either<TLeft, TNewRight>> MapRightAsync<TLeft, TRight, TNewRight>(
		this Either<TLeft, TRight> either,
		Func<TRight, Task<TNewRight>> mapper) =>
		either.IsRight
			? Either<TLeft, TNewRight>.FromRight(await mapper(either.Right))
			: Either<TLeft, TNewRight>.FromLeft(either.Left);

	/// <summary>
	/// Asynchronously binds the Right value of an Either instance to a function that returns a Task of a new Either,
	/// effectively allowing to chain multiple asynchronous operations that might fail.
	/// </summary>
	/// <typeparam name="TLeft">The type of the Left value.</typeparam>
	/// <typeparam name="TRight">The type of the source Right value.</typeparam>
	/// <typeparam name="TNewRight">The type of the new Right value.</typeparam>
	/// <param name="either">The Either instance to bind.</param>
	/// <param name="binder">The async function that transforms the Right value into a new Either.</param>
	/// <returns>
	/// A task representing the asynchronous operation, with the result of applying the binder function
	/// to the Right value if the instance contains a Right value; otherwise, a new Either containing
	/// the original Left value.
	/// </returns>
	/// <example>
	/// <code>
	/// // Chain multiple async operations that might fail
	/// var result = await GetUserAsync(id)
	///     .BindAsync(async user => await GetUserOrdersAsync(user.Id))
	///     .BindAsync(async orders => await ProcessOrdersAsync(orders));
	/// </code>
	/// </example>
	public static async Task<Either<TLeft, TNewRight>> BindAsync<TLeft, TRight, TNewRight>(
		this Either<TLeft, TRight> either,
		Func<TRight, Task<Either<TLeft, TNewRight>>> binder) =>
		either.IsRight
			? await binder(either.Right)
			: Either<TLeft, TNewRight>.FromLeft(either.Left);

	/// <summary>
	/// Safely executes an asynchronous function and wraps the result in an Either,
	/// capturing any exceptions in the Left side.
	/// </summary>
	/// <typeparam name="TResult">The return type of the function to execute.</typeparam>
	/// <param name="func">The asynchronous function to execute safely.</param>
	/// <returns>
	/// A task representing the asynchronous operation, with an Either containing either
	/// the exception in the Left side or the function result in the Right side.
	/// </returns>
	/// <example>
	/// <code>
	/// // Safely execute an API call that might throw exceptions
	/// var result = await EitherAsyncExtensions.TryAsync(
	///     async () => await apiClient.FetchDataAsync()
	/// );
	/// 
	/// // Process the result safely
	/// await result.MatchAsync(
	///     async ex => await LogExceptionAsync(ex),
	///     async data => await ProcessDataAsync(data)
	/// );
	/// </code>
	/// </example>
	public static async Task<Either<Exception, TResult>> TryAsync<TResult>(Func<Task<TResult>> func)
	{
		try
		{
			return Either<Exception, TResult>.FromRight(await func());
		}
		catch (Exception ex)
		{
			return Either<Exception, TResult>.FromLeft(ex);
		}
	}
}