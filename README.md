# About

This is a very simple library I built for more easily interactive with the `System.Text.Json` library built into newer versions of .Net

## Examples

`System.Text.Json` is great about working with Serializing and Deserializing between C# Objects and `JsonObject` / `JsonArray` / `JsonNode` / etc; but not always so straight forward when you just want to do simple JSON access.

In vanilla `System.Text.Json` you may do something like:

```csharp
JsonObject obj = new JsonObject();
if (
    obj.TryGetPropertyValue("key", out JsonNode? node)
    && node is JsonObject obj2
    && obj2.TryGetPropertyValue("key2", out JsonNode2? node)
) {
    // ... Do stuff here
}
```

`JsonObject` has one built-in `TryGetValue` Method which outs a `JsonNode?`, which could be a `JsonValue<T>`, `JsonArray`, or `JsonObject`.

Here instead we can do

```csharp
JsonObject obj = new JsonObject();
if (
    obj.TryGetPropertyValue("key", out JsonObject? obj2)
    && obj2.TryGetPropertyValue("key2", out JsonObject? ob3)
) {
    // ... Do stuff here
}
```

We can also easily out values from `JsonValue<T>`

```csharp
JsonObject obj = new();
obj.TryGetPropertyValue("key", out string _);
obj.TryGetPropertyValue("key", out bool _);
obj.TryGetPropertyValue("key", out long _);
obj.TryGetPropertyValue("key", out ulong _);
obj.TryGetPropertyValue("key", out int _);
obj.TryGetPropertyValue("key", out uint _);
obj.TryGetPropertyValue("key", out Guid _);
obj.TryGetPropertyValue("key", out DateTimeOffset _);
```