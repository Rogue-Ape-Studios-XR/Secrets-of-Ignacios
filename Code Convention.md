# Code Convention

- [Classes](#classes)
- [Fields](#fields)
- [Serialized Fields](#serialized-fields)
- [Local Variables](#local-variables)
- [Constants](#constants)
- [Properties](#properties)
- [Methods](#methods)
- [Events](#events)
- [Interfaces](#interfaces)
- [Singletons](#singletons)
- [Enumeration](#enumeration)
- [Structs](#structs)
- [Structure](#structure)
- [Namespaces](#namespaces)
- [Style](#style)
- [Monobehaviour](#monobehaviour)
- [Asynchronus Programming](#asynchronous-programming)
- [Miscellaneous Changes](#miscellaneous-changes)
- [Unitask](#unitask)


## Classes

Names will always be in PascalCase.

``` C#
// Not allowed:
class carFactory {...}
class car_Factory {...}

// Is Allowed:
class CarFactory {...}

```

## Fields

Fields are prefixed by an underscore, and are spelled in camelCase. If you need a field to be visible in the editor use `[SerializeField]`.

``` C#
private int _age;
[SerializeField] private string _localAdress;
```

Protected fieds should be avoided, whereas public and internal fields should not exist at all.
Only in extreme circumstances should they be allowed, but their existence should be explained. In this case, they are spelled in pure camelCase (no underscore).

``` C#
public int count;
```

## Serialized Fields

Serialisation using `[SerializedField]` must be done on the same line as the field name, for clarity. Additional Attributes, such as range, can be defined as `[SerializedField, Range(0,10)]`.

Additionally, they must always be private.

## Local Variables

Local variables should be in pure camelCase (no underscore).

``` C#
private float GetArea(float height, float width)
{
    int length = 5;

    return height * width * length;
}
```

## Constants

Contants should be written in PascalCase.

``` C#
public constant string EnemyName = "BulletBill";
```

## Properties

Properties will be accessed externally, and require clear naming. Use Pascal case for these.

``` C#
public float TimeLeft {get; private set;}
```

Additionally, if they return a field, they should be written as follows:

``` C#
public float TimeLeft => _timeLeft;
```

## Methods

All methods are written in PascalCase, disregarding the modifier. By default, methods must be private, unless required outside the class or assembly.

``` C#
public float GetEnemyByDifficulty(Difficulty difficulty) {...}
```

## Events

Events should be prefaced with the `on` prefix, as long as this makes sense.

``` C#
public event Action<bool> onClick;
```

Methods can (but are not required to) use the `Handle` prefix, followed by the event name.

``` C#
// Optional:
public void HandleButtonClick(bool state) {...}
```

## Interfaces

Preface an interface name with `I`. This is a widespread practice within the C# community, and ignoring it may do more harm than good.

## Singletons

Should be avoided, but may be required in specific use cases.

## Enumeration

Should always have an accessor, and written as a Singular as well as Pascal Case.
They have their own file.

``` C#
internal enum Colour
{
    Red,
    Green,
    Blue
}
```

## Structs

Are treated in the same way as classes, and thus must be written in PascalCase.

## Structure
A basic structure would be as following.

```C#
class Example : Monobehaviour
{
    // Fields

    // Properties

    // Setup, such as Awake, Start, OnDestroy

    // Update, such as Update, FixedUpdate

    // Methods
}
```

Never place fields, properties, etc. in between the setup, update or methods.

## Namespaces

Namespaces should use the following style:

``` C#
namespace OrganistationName.ProjectName.Folder...;
```

Where folder is the folder the code lives in. This also acts as an overal category, IE `Score`, `Player`, `Melee`.

Sub folders are encouraged to a degree, though the majority of code should not live under `Gameplay.Folder`. Instead, `UI` and `Audio` can live in the same folder as `Score`.

If Assembly Definitions are being used, they are the first thing that should be added to a folder, this way the namespace in this definition will be automatically added to the script.

## Style

### Avoid

- Using `this` if you don't need to. Your IDE will usually understand if you are trying to access it's own property or method. If you use another copy of the same class in the class, feel free to use `this`.

- For readability sake, wrap and align functions when needed. There is no specific request when it comes to wrapping.
  
``` C#
Instantiate(prefab,
            transform.position
            Quaternion.Identity,
            null);
```

### Encouraged

- When using a switch statement, add a default. This can always be used for a `NotImplementedException`, allowing the next developer to know they need to add addtional cases.

- When using a switch statement, a [Switch Expression](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/switch-expression) can be used if you are simply pattern matching. Visual Studio usually suggests this by itself, if it can be applied.

- Drop the second type specification if one already exists:

``` C#
// No:
private Gun _railGun = new Gun();

// Yes:
private Gun _railGun = new();
```

## MonoBehaviour

- If you access a Unity Property (like [Transform](https://docs.unity3d.com/ScriptReference/Transform.html)), make a reference to the transform and use that. Unity has to perform some calls under the hood, which adds unnecessary overhead.

- Instantiate a new object by referencing it's script, if you need the script later in your code.
This is done by creating a copy of the object in your scene, and referencing that object as the prefab.

``` C#
[SerializeField] private ColliderController _colliderController;

private void SpawnEnemy()
{
    var enemy = Instantiate(_colliderController, transform, null);
    enemy.DisableCollider(); // This works.
}
```

- Avoid using `GetComponent`. It's an expensive operation, serialize a reference instead. This reduces the wake-up time, and improves the speed of the code.

## Asynchronous Programming

- Favour `async` over `Coroutines`. There are several advantages:
  - It can be invoked from any method marked `async`, instead of only `Monobehaviour` classes.
  - It provides better error handling (`yield` cannot be declared in `try/catch/finally`).
  - It can return values when awaited, eliminating the need for callbacks.
  - It can be easily chained.
  - It does not require running on `Timing.PlayerLoop`, allowing other methods to take advantage of the timing.
  - It has cancellation support, allowing the use of `CancellationTokens`. This reduces risk, as opposed to Coroutines.

- Favour UniTask over C# vanilla Tasks. It has fewer GC allocations, and has some useful Unity   specific API calls.

- Methods marked `async` should always be awaited, unless:
      - They are event handlers. In that case, use `async void`.
      - They are unity event methods, such as `Awake` or `Start`. In that case, use `async void`.

- Avoid `UniTask.Forget`. Methods should mostly be awaited.

- Calls to `CancellationTokenSource.Cancel()` should always be followed by `CancellationTokenSource.Dispose()`.

- Append an `async` suffix to any `async` method for clarity.

- Asynchronous methods that should be awaited must support the use of a CancellationToken.
      - If private or protected, make sure that there is an `OnDestroy` method which cancels the token.
      - If public or internal, the method should accept a `CancellationToken` as a parameter.

- Top most Asynchronous methods should wrap the call in a `try/catch`, with a catch for **at least** `OperationCancelledException`, explaining what was cancelled and why. For example: "Connection attempt was cancelled because the operation was cancelled."

## Miscellaneous Changes

- Avoid `LayerMask.GetMask`. Instead, serialzie a `LayerMask`. This avoids potential mistakes and is slightly better for performance.

- Avoid using Unity events, due to them making debugging significantly harder, as they hide subscribers. Prefer vanilla C# events. This may not be possible for some features, because of the packages that are being used.

- Do not trust the scene hieracrhy, as this can change significantly over the course of gameplay. If you need to reference a parent or child class, serialzie a reference. Common names are `_root` or `_parent`.

- When raycasting, always consider the maxDistance. This improves performance significantly, as less calculations need to be made.
    - Never use `MathF.Infinity`.

- `FindObjectOfType` and any other general purpose searching methods should be avoided at all costs. If there truly is no other way, you should explain in comments why.

## Unitask
If you are using unitask make sure it is implemented like this:

```c#
private CancellationTokenSource _cancellationTokenSource;

private void Awake()
{
     _cancellationTokenSource = new CancellationTokenSource();
}

private void OnDestroy()
{
    _cancellationTokenSource.Cancel();
    _cancellationTokenSource.Dispose();
}

 private async void Example(CancellationToken token)
{
    try
    {
        // Do stuff before timer starts
        // WaitForSeconds is used only as an example
        await UniTask.WaitForSeconds(1 , cancellationToken: token);
        // Do stuff after timer is finished         
    }
    catch (OperationCanceledException)
    {
        Debug.LogError("Example was Canceled...");
    }
}

```

