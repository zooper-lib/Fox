# Fox

<img src="icon.png" alt="drawing" width="256"/>

A lightweight functional programming library for C# that provides monadic types like Either, Option, and Unit.

## 🚀 Overview
Fox brings functional programming patterns to C# with a simple, intuitive API. The library focuses on providing algebraic data types that help with error handling, optional values, and composable operations.

## 📦 Installation
```bash
dotnet add package Zooper.Fox
```

## 🔧 Usage

### Either Type
Either is a disjoint union of two types, useful for representing values that can be one of two possible types.

```csharp
using Zooper.Fox;

// Create Either instances
var success = Either<string, int>.FromRight(42);
var error = Either<string, int>.FromLeft("Something went wrong");

// Pattern matching
var result = success.Match(
    left => $"Error: {left}",
    right => $"Success: {right}"
);

// Implicit conversion
Either<string, int> implicitSuccess = 42;
Either<string, int> implicitError = "Error occurred";
```

### Option Type
Option represents a value that may or may not exist.

```csharp
using Zooper.Fox;

// Create Option instances
var some = Option<int>.Some(42);
var none = Option<int>.None();

// Check if value exists
if (some.IsSome)
{
    Console.WriteLine(some.Value);
}

// Pattern matching with Option
var message = some.Match(
    value => $"Got value: {value}",
    () => "No value"
);
```

### LINQ Extensions
Use familiar LINQ patterns with Either and Option types:

```csharp
var result = await GetUserAsync(userId)
    .BindAsync(user => GetOrdersAsync(user.Id))
    .MapAsync(orders => orders.Count);
```

## 🤝 Contributing
Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Made with ❤️ by the Zooper team
