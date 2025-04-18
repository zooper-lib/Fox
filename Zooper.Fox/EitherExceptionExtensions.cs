using System;
using System.Threading.Tasks;

namespace Zooper.Fox;

/// <summary>
/// Provides extension methods for working with exceptions in a functional way using Either types.
/// <para>
/// These utilities allow you to safely execute operations that might throw exceptions,
/// capturing any exceptions and returning them as part of an Either instance. This helps
/// move from exception-based error handling to a more functional approach.
/// </para>
/// </summary>
public static class EitherExceptionExtensions
{
	/// <summary>
	/// Safely executes a function and wraps the result in an Either, capturing any exceptions in the Left side.
	/// <para>
	/// This is especially useful when calling code that wasn't designed with functional error
	/// handling in mind and might throw exceptions.
	/// </para>
	/// </summary>
	/// <typeparam name="TResult">The return type of the function to execute.</typeparam>
	/// <param name="func">The function to execute safely.</param>
	/// <returns>
	/// An Either containing either the exception in the Left side or the function result in the Right side.
	/// </returns>
	/// <example>
	/// <code>
	/// // Safely execute a function that might throw
	/// var result = EitherExceptionExtensions.Try(() => int.Parse(userInput));
	/// 
	/// // Handle the result safely
	/// string message = result.Match(
	///     ex => $"Error parsing input: {ex.Message}",
	///     value => $"Successfully parsed value: {value}"
	/// );
	/// </code>
	/// </example>
	public static Either<Exception, TResult> Try<TResult>(Func<TResult> func)
	{
		try
		{
			return Either<Exception, TResult>.FromRight(func());
		}
		catch (Exception ex)
		{
			return Either<Exception, TResult>.FromLeft(ex);
		}
	}

	/// <summary>
	/// Safely executes an asynchronous function and wraps the result in an Either,
	/// capturing any exceptions in the Left side.
	/// <para>
	/// This is especially useful when calling asynchronous code that wasn't designed 
	/// with functional error handling in mind and might throw exceptions.
	/// </para>
	/// </summary>
	/// <typeparam name="TResult">The return type of the asynchronous function to execute.</typeparam>
	/// <param name="func">The asynchronous function to execute safely.</param>
	/// <returns>
	/// A task representing the asynchronous operation, with an Either containing either
	/// the exception in the Left side or the function result in the Right side.
	/// </returns>
	/// <example>
	/// <code>
	/// // Safely execute an async API call
	/// var result = await EitherExceptionExtensions.TryAsync(
	///     async () => await httpClient.GetStringAsync(url)
	/// );
	/// 
	/// // Process the result safely
	/// await result.MatchAsync(
	///     async ex => await LogExceptionAsync(ex),
	///     async content => await ProcessContentAsync(content)
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