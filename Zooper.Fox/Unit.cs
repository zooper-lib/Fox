namespace Zooper.Fox;

/// <summary>
/// Represents a type with only one possible value, similar to void but usable as a type parameter.
/// <para>
/// Unit is a fundamental type in functional programming that represents the absence of a specific value.
/// It's often used when a function must return something but there is no meaningful value to return,
/// or as a placeholder type in generic contexts.
/// </para>
/// 
/// <para>
/// Unlike void in C#, Unit can be used as a generic type parameter, making it useful in monadic types
/// like <see cref="Either{TLeft, TRight}"/> and <see cref="Option{T}"/>.
/// </para>
/// 
/// <example>
/// <code>
/// // Used as a placeholder in Either when there's no meaningful error value
/// Either&lt;Unit, int&gt; GetNumber() => Either&lt;Unit, int&gt;.FromRight(42);
/// 
/// // Used in Option to represent None case
/// Option&lt;string&gt; name = Option&lt;string&gt;.None(); // Internally uses Unit
/// </code>
/// </example>
/// </summary>
public struct Unit
{
	/// <summary>
	/// Gets the singleton instance of the Unit type.
	/// <para>
	/// Since Unit has only one possible value, this property provides access to that value.
	/// </para>
	/// </summary>
	/// <value>The singleton Unit value.</value>
	/// <example>
	/// <code>
	/// Unit unit = Unit.Value;
	/// </code>
	/// </example>
	public static Unit Value => new();
}