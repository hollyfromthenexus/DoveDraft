using System;
using System.Collections.Generic;
using System.Numerics;

namespace DoveDraft;

/// <summary>
/// An interface which represents some value, controlled by a collection of preferences ordered by priority.
/// </summary>
/// <typeparam name="TKey">The type to use for preference keys.</typeparam>
/// <typeparam name="TValue">The type of the value to actually store.</typeparam>
public interface IPrioritizedPref<TKey, TValue>
{
    /// <summary>
    /// A method signature used for <see cref="OnValueChanged"/>.
    /// </summary>
    /// <param name="newValue">The new contents of <see cref="Value"/>.</param>
    /// <param name="prefKey">The key of the preference driving the new value.</param>
    public delegate void PrioritizedPrefValueChangedDelegate(TValue newValue, TKey prefKey);

    /// <summary>
    /// A single preference for what <see cref="IPrioritizedPref{TKey, TValue}.Value"/> should be.
    /// </summary>
    /// <param name="Key">A key to represent this preference.</param>
    /// <param name="Value">The value that this preference stands for.</param>
    /// <param name="Priority">The priority of this preference. Higher = top priority, will overrule lower values.</param>
    public record struct Preference(TKey Key, TValue Value, int Priority);

    /// <summary>
    /// Invoked upon the contents of <see cref="Value"/> changing.
    /// </summary>
    public event PrioritizedPrefValueChangedDelegate OnValueChanged;

    /// <summary>
    /// Gets the current preferred value.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Gets a collection of all preferences for <see cref="Value"/>.
    /// </summary>
    public IEnumerable<Preference> AllPreferences { get; }

    /// <summary>
    /// Apply some preference to what <see cref="Value"/> should be. Will add it to <see cref="AllPreferences"/> if the key
    /// doesn't already exist in the collection. Will overwrite an existing preference if the key DOES already exist.
    /// </summary>
    /// <param name="key">The key of the preference.</param>
    /// <param name="value">The value this preference stands for.</param>
    /// <param name="priority">The priority of this preference. Higher will overrule lower priority preferences.</param>
    public void SetPreference(TKey key, TValue value, int priority);

    /// <summary>
    /// Removes some preference from what <see cref="Value"/> should be, if one for the given key already exists.
    /// </summary>
    /// <param name="key">The key of the preference to clear.</param>
    public void ClearPreference(TKey key);

    /// <summary>
    /// Does this value have a preference by the given key?
    /// </summary>
    /// <param name="key">The key of the preference to look for.</param>
    /// <returns>True if there's a preference by this key, false otherwise.</returns>
    public bool HasPreference(TKey key);

    /// <summary>
    /// Attempt to get a specific preference for <see cref="Value"/>, if one exists by the given key.
    /// </summary>
    /// <param name="key">The key of the preference to get.</param>
    /// <param name="preference">Where to store the located preference, if one exists.</param>
    /// <returns>True if a preference was found for the given key, false otherwise.</returns>
    public bool TryGetPreference(TKey key, out Preference preference);

    /// <summary>
    /// Remove all preferences for what <see cref="Value"/> should be.
    /// </summary>
    public void ClearAllPreferences();
}

/// <summary>
/// A concrete implementation of <see cref="IPrioritizedPref{TKey, TValue}"/>, backed by a sorted list.
/// </summary>
/// <typeparam name="TKey">The type to use for preference keys.</typeparam>
/// <typeparam name="TValue">The type of the value to actually store.</typeparam>
public sealed class PrioritizedPref<TKey, TValue> : IPrioritizedPref<TKey, TValue>
{
    //
    //  Public Events
    //

    /// <inheritdoc/>
    public event IPrioritizedPref<TKey, TValue>.PrioritizedPrefValueChangedDelegate OnValueChanged;

    //
    //  Public Data
    //

    /// <inheritdoc/>
    public TValue Value
    {
        get => internalValue;
        set
        {
            bool valueChanged = !value.Equals(internalValue);

            if (valueChanged)
            {
                internalValue = value;

                if (OnValueChanged != null)
                {
                    TKey key =
                        sortedPreferences.Count == 0
                            ? default
                            : sortedPreferences.GetValueAtIndex(sortedPreferences.Count - 1).Key;

                    OnValueChanged.Invoke(value, key);
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets what the contents of <see cref="Value"/> should be, when there's no preference.
    /// </summary>
    public TValue DefaultValue { get; set; }

    /// <inheritdoc/>
    public IEnumerable<IPrioritizedPref<TKey, TValue>.Preference> AllPreferences =>
        sortedPreferences.Values;

    //
    //  Private Data
    //

    private TValue internalValue;
    private SortedList<
        ValueTuple<int, TKey>,
        IPrioritizedPref<TKey, TValue>.Preference
    > sortedPreferences = new();

    //
    //  Constructors
    //

    public PrioritizedPref(TValue defaultValue = default)
    {
        DefaultValue = defaultValue;
    }

    //
    //  Public Methods
    //

    /// <summary>
    /// Apply some preference to what <see cref="Value"/> should be. Will add it to <see cref="AllPreferences"/> if the key
    /// doesn't already exist in the collection. Will overwrite an existing preference if the key DOES already exist.
    /// </summary>
    /// <param name="key">The key of the preference.</param>
    /// <param name="value">The value this preference stands for.</param>
    /// <param name="priority">The priority of this preference. Higher will overrule lower priority preferences.</param>
    public void SetPreference(TKey key, TValue value, int priority)
    {
        // If we already have a preference for this key, remove it.
        int index = IndexOfPrefKey(key);
        if (index != -1)
        {
            sortedPreferences.RemoveAt(index);
        }

        // Add the new preference to the collection.
        sortedPreferences.Add(
            (priority, key),
            new IPrioritizedPref<TKey, TValue>.Preference(key, value, priority)
        );
        RefreshValue();
    }

    /// <summary>
    /// Removes some preference from what <see cref="Value"/> should be, if one for the given key already exists.
    /// </summary>
    /// <param name="key">The key of the preference to clear.</param>
    public void ClearPreference(TKey key)
    {
        int index = IndexOfPrefKey(key);
        if (index != -1)
        {
            sortedPreferences.RemoveAt(index);
        }

        RefreshValue();
    }

    /// <summary>
    /// Does this value have a preference by the given key?
    /// </summary>
    /// <param name="key">The key of the preference to look for.</param>
    /// <returns>True if there's a preference by this key, false otherwise.</returns>
    public bool HasPreference(TKey key) => IndexOfPrefKey(key) != -1;

    /// <summary>
    /// Attempt to get a specific preference for <see cref="Value"/>, if one exists by the given key.
    /// </summary>
    /// <param name="key">The key of the preference to get.</param>
    /// <param name="preference">Where to store the located preference, if one exists.</param>
    /// <returns>True if a preference was found for the given key, false otherwise.</returns>
    public bool TryGetPreference(TKey key, out IPrioritizedPref<TKey, TValue>.Preference preference)
    {
        int index = IndexOfPrefKey(key);
        if (index == -1)
        {
            preference = default;
            return false;
        }

        preference = sortedPreferences.GetValueAtIndex(index);
        return true;
    }

    /// <summary>
    /// Remove all preferences for what <see cref="Value"/> should be.
    /// </summary>
    public void ClearAllPreferences()
    {
        sortedPreferences.Clear();
        RefreshValue();
    }

    //
    //  Private Methods
    //

    /// <summary>
    /// Maintains the contents of value by reading <see cref="sortedPreferences"/>. Should be called
    /// whenever sortedPreferences is maintained.
    /// </summary>
    private void RefreshValue()
    {
        if (sortedPreferences.Count == 0)
        {
            Value = DefaultValue;
            return;
        }

        Value = sortedPreferences.GetValueAtIndex(sortedPreferences.Count - 1).Value;
    }

    /// <summary>
    /// Find the first index of the given key in the internal <see cref="sortedPreferences"/> collection.
    /// </summary>
    /// <param name="keyToFind">The key to find.</param>
    /// <returns>The index of the given key if found, -1 otherwise.</returns>
    private int IndexOfPrefKey(TKey keyToFind)
    {
        int i = 0;
        foreach ((int priority, TKey key) in sortedPreferences.Keys)
        {
            if (key.Equals(keyToFind))
            {
                return i;
            }

            i++;
        }

        return -1;
    }
}
