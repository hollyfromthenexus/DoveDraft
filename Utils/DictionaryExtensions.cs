using System.Collections.Generic;
using Godot;

namespace DoveDraft;

public static class DictionaryExtensions
{
    public static Godot.Collections.Dictionary<TKey, TValue> ToGodot
        <[MustBeVariant] TKey, [MustBeVariant] TValue>
        (this Dictionary<TKey, TValue> dictionary)
    {
        return new Godot.Collections.Dictionary<TKey, TValue>(dictionary);
    }

    public static Dictionary<TKey, TValue> ToCSharp
        <[MustBeVariant] TKey, [MustBeVariant] TValue>
        (this Godot.Collections.Dictionary<TKey, TValue> dictionary)
    {
        return new Dictionary<TKey, TValue>(dictionary);
    }
}
