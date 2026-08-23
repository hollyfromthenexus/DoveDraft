using System.Collections.Generic;
using Godot;

namespace DoveDraft;

public partial class MouseLock : Node, IMouseLockService
{
    //
    //  Public Events
    //

    [Signal]
    public delegate void StateChangedEventHandler(bool isLocked);

    //
    //  Public Data
    //

    /// <inheritdoc/>
    public bool IsLocked => lockPreference.Value;

    //
    //  Private Data
    //

    private readonly PrioritizedPref<string, bool> lockPreference = new();

    private bool IsActualMouseLocked
    {
        get => Input.MouseMode == Input.MouseModeEnum.Captured;
        set => Input.MouseMode = value ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
    }

    //
    //  Constructor
    //

    public MouseLock()
    {
        lockPreference.OnValueChanged += OnLockPreferenceValueChanged;
    }

    //
    //  Godot Methods
    //

    /// <inheritdoc/>
    public override void _EnterTree()
    {
        Services.Register<IMouseLockService>(this);
    }

    /// <inheritdoc/>
    public override void _ExitTree()
    {
        Services.Unregister<IMouseLockService>();
        lockPreference.ClearAllPreferences();
    }

    /// <inheritdoc/>
    public override void _Input(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton clickEvent:
                if (clickEvent.Pressed && clickEvent.ButtonIndex == MouseButton.Left)
                {
                    ClearLock(nameof(MouseLock));
                }
                break;

            case InputEventKey keyEvent:
                if (keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
                {
                    SetLock(nameof(MouseLock), false, 500);
                }
                break;
        }
    }

    //
    //  IMouseLockService Methods
    //

    /// <inheritdoc/>
    public void SetLock(string key, bool isLocked, int priority) =>
        lockPreference.SetPreference(key, isLocked, priority);

    /// <inheritdoc/>
    public void ClearLock(string key) => lockPreference.ClearPreference(key);

    //
    //  Private Methods
    //

    /// <summary>
    /// Glue. Makes it so that the state of <see cref="lockPreference"/> controls the
    /// actual mouse lock state. Also invokes events as necessary.
    /// </summary>
    /// <param name="isLocked">The new mouse lock state.</param>
    /// <param name="prefKey">The key driving the new mouse lock state.</param>
    private void OnLockPreferenceValueChanged(bool isLocked, string prefKey)
    {
        Log.For<MouseLock>(
            $"Mouse {(isLocked ? "locked" : "unlocked")} by \"{prefKey ?? "DEFAULT"}\"."
        );
        IsActualMouseLocked = isLocked;
        EmitSignalStateChanged(isLocked);
    }
}

public partial class Services
{
    public static IMouseLockService MouseLock => Get<IMouseLockService>();
}
