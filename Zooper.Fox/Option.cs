using System;

// ReSharper disable MemberCanBePrivate.Global

namespace Zooper.Fox;

/// <summary>
/// Represents an optional value that may or may not be present.
/// <para>
/// The Option type is a specialized version of <see cref="Either{TLeft, TRight}"/> where:
/// - The Left side is <see cref="Unit"/>, representing no value (None)
/// - The Right side is the actual value (Some)
/// </para>
/// 
/// <para>
/// Use Option when a value might be absent (null) to make the possibility of absence explicit in the type system.
/// This helps prevent null reference exceptions and makes code more readable and maintainable.
/// </para>
/// 
/// <example>
/// <code>
/// // Create a Some value
/// var userName = Option&lt;string&gt;.Some("JohnDoe");
/// 
/// // Create a None value
/// var emptyUser = Option&lt;string&gt;.None();
/// 
/// // Pattern match to safely handle both cases
/// string display = userName.Match(
///     value => $"Hello, {value}!",
///     () => "Hello, guest!"
/// );
/// </code>
/// </example>
/// </summary>
/// <typeparam name="T">The type of the optional value.</typeparam>
public class Option<T> : Either<Unit, T>
{
	private static readonly Unit _none = new();

	/// <summary>
	/// Initializes a new Option instance containing a value (Some).
	/// This constructor is private - use <see cref="Some"/> factory method instead.
	/// </summary>
	/// <param name="value">The value to wrap.</param>
	private Option(T value) : base(value) { }

	/// <summary>
	/// Initializes a new Option instance representing no value (None).
	/// This constructor is private - use <see cref="None"/> factory method instead.
	/// </summary>
	private Option() : base(_none) { }

	/// <summary>
	/// Creates a new Option instance containing a value (Some).
	/// </summary>
	/// <param name="value">The value to wrap in the Option.</param>
	/// <returns>A new Option instance representing a present value.</returns>
	/// <example>
	/// <code>
	/// Option&lt;int&gt; maybeId = Option&lt;int&gt;.Some(42);
	/// </code>
	/// </example>
	public static Option<T> Some(T value) => new Option<T>(value);

	/// <summary>
	/// Creates a new Option instance representing no value (None).
	/// </summary>
	/// <returns>A new Option instance representing an absent value.</returns>
	/// <example>
	/// <code>
	/// Option&lt;string&gt; emptyName = Option&lt;string&gt;.None();
	/// </code>
	/// </example>
	public static Option<T> None() => new Option<T>();

	/// <summary>
	/// Indicates whether this Option contains a value (Some).
	/// </summary>
	/// <value>true if this Option contains a value; otherwise, false.</value>
	public bool IsSome => IsRight;

	/// <summary>
	/// Indicates whether this Option does not contain a value (None).
	/// </summary>
	/// <value>true if this Option does not contain a value; otherwise, false.</value>
	public bool IsNone => IsLeft;

	/// <summary>
	/// Gets the value contained in this Option.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Thrown when attempting to access the Value when this instance is None.
	/// Always check <see cref="IsSome"/> before accessing this property.
	/// </exception>
	/// <value>The contained value if present; otherwise throws an exception.</value>
	/// <example>
	/// <code>
	/// var maybeUser = GetUserById(id);
	/// if (maybeUser.IsSome)
	/// {
	///     Console.WriteLine($"Found user: {maybeUser.Value.Name}");
	/// }
	/// </code>
	/// </example>
	public T Value => IsSome
		? Right!
		: throw new InvalidOperationException("Cannot access Value on None.");
		
	/// <summary>
	/// Matches the current Option to one of two possible functions based on whether
	/// a value is present (Some) or absent (None), and returns the result.
	/// </summary>
	/// <typeparam name="TResult">The return type of the matching functions.</typeparam>
	/// <param name="someFunc">The function to invoke if a value is present.</param>
	/// <param name="noneFunc">The function to invoke if a value is absent.</param>
	/// <returns>The result of the invoked function.</returns>
	/// <example>
	/// <code>
	/// string message = userOption.Match(
	///     user => $"User found: {user.Name}",
	///     () => "No user found"
	/// );
	/// </code>
	/// </example>
	public TResult Match<TResult>(Func<T, TResult> someFunc, Func<TResult> noneFunc)
	{
		return IsSome
			? someFunc(Value)
			: noneFunc();
	}
}