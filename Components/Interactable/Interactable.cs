using Godot;

public interface IInteractable
{
    public bool IsInteractable { get; }

    public bool IsHovering { get; set; }

    public bool IsUsing { get; set; }
}

[GlobalClass]
public abstract partial class Interactable : Node, IInteractable
{
    //
    //  Public Data
    //

    public abstract bool IsInteractable { get; }

    public bool IsHovering
    {
        get => isHoveringInternal;
        set
        {
            if (isHoveringInternal == value) return;

            isHoveringInternal = value;
            if (isHoveringInternal)
            {
                HoverStart();
            }
            else
            {
                HoverStop();
            }
        }
    }
    private bool isHoveringInternal;

    public bool IsUsing
    {
        get => isUsingInternal;
        set
        {
            if (isUsingInternal == value) return;

            isUsingInternal = value;
            if (isUsingInternal)
            {
                UseStart();
            }
            else
            {
                UseStop();
            }
        }
    }
    private bool isUsingInternal;

    //
    //  Protected Methods
    //

    protected virtual void UseStart() { /* no-op */ }

    protected virtual void UseStop() { /* no-op */ }

    protected virtual void HoverStart() { /* no-op */ }

    protected virtual void HoverStop() { /* no-op */ }
}
