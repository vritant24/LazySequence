# LazySequence

[![.NET](https://github.com/vritant24/LazySequence/actions/workflows/dotnet.yml/badge.svg)](https://github.com/vritant24/LazySequence/actions/workflows/dotnet.yml)

[Add the Nuget Package to your project](https://www.nuget.org/packages/LazySequence/)

## What is LazySequence?

It is an abstraction built on top of `IEnumerable` to lazily (and maybe even asynchronously) generate sequences with low friction.

## Why was it made?

Because I had an idea and thought it would be fun to make it
Inspired by lazy evaluation in functional languages, like Haskell, that allow "infinite" data structures, I set out to be able to do so in C# after learning how `yield` works.

## How do you use it?

1. Take a look at the [src\LazySequence](src\LazySequence) to see the documented public API.
2. All examples can be found in [samples/Samples](samples/Samples).

### A contrived example

Taking an example from Math, consider an arithmetic progression:

```cs
// Create a sequence of all positive integers.
IEnumerable<int> allPositiveIntegers = LazySequence<int>.Create(
    firstElement: 1,
    (prev, index) => (prev + 1, false));
```

`LazySequence` allows you to create a sequence through an 'equation' where you specify the first element, and a function to compute every successive element.

In this case:

1. The first element is `1`.
2. We provide a method that takes the previous element, and the index of the element we need to return. We return a tuple of:
    1. The next element, which here is `prev + 1`.
    2. A bool to indicate whether the sequence has ended, here we do not want the sequence to end, so we always return `false`.

Why is it cool? Because `allPositiveIntegers` above is now a never-ending* enumerable! (*restricted by `ulong` and memory). Which means it can be used the following way:

```cs
foreach (var element in allPositiveIntegers)
{
    // Don't really do this unless you want to see a stack overflow.
    // It's a never-ending data structure after all.
    Console.WriteLine(element);
}
```

And since it is an `IEnumerable`, we can also take advantage of `LINQ`

```cs
// sum of first 100 positive integers
var sumOfFirstHundredIntegers = allPositiveIntegers
    .Take(100)
    .Aggregate((acc, el) => acc + el);
```

### Practical use

As cool as arithmetic and geometric progressions may be, they don't really appear that commonly in day-to-day work sadly :(

But Lazy sequence can be used in more practical scenarios as well! With the ability to pass state along with the enumeration, it can be used as far as your imagination can take you!

A couple to highlight:

#### Object generation

[See samples\Samples/ObjectGeneration.cs](samples\Samples\ObjectGeneration.cs)

```cs
/*
 * Consider a scenario where you need to generate unique names.
 */
IEnumerable<string> uniqueNames = LazySequence<string>.Create(
    string.Empty,
    (prev, idx) => ($"name_{idx}", false));
```

#### Pagination

[See samples\Samples\AsyncSample.cs](samples\Samples\AsyncSample.cs)

```cs
/*
 * An AsyncLazySequence can be used as a medium to lazily paginate
 * over web requests to your API.
 */

var pageSize = 10;
IAsyncEnumerable<object?> paginatedServerRequestCreator = AsyncLazySequence<object?, int>.Create(
    firstElement: null,
    initialState: 0,
    async (prev, currentOffset, index) =>
    {
        var serverResponse = await ServerPageRequest(currentOffset, pageSize);
        return (
            nextElement: serverResponse,
            iterationState: currentOffset + pageSize,
            isLastElement: false);
    });
```

## Got Ideas to improve this?

Open an issue! I'm very open to feedback. I made this as a learning project, but if this can be made into a tool worth using, I'm willing to put in the work :)
