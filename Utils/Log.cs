using System.Runtime.CompilerServices;
using Godot;

public static class Log
{
    public static void For<T>(string message)
    {
        GD.Print($"[{typeof(T)}] {message}");
    }
}
