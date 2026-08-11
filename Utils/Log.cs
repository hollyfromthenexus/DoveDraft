using Godot;

namespace DoveDraft;

public static class Log
{
    public static void For<T>(string message)
    {
        GD.Print($"[{typeof(T)}] {message}");
    }

    public static void WarnFor<T>(string message)
    {
        GD.PushWarning($"[{typeof(T)}] {message}");
    }
}
