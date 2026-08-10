using Godot;
using System.Collections.Generic;

namespace DoveDraft;

public interface IPlayerInteractor
{
    public bool IsUsing { get; set; }

    public bool IsHovering { get; }

    public GodotObject FocusedObject { get; }

    public List<Interactable> FocusedInteractables { get; }
}

public partial class PlayerInteractor : RayCast3D, IPlayerInteractor
{
    //
    //  Signals
    //

    [Signal]
    public delegate void IsHoveringChangedEventHandler(bool isHovering);

    //
    //  Public Data
    //

    public bool IsUsing
    {
        get => isUsingInternal;
        set
        {
            if (value == isUsingInternal) return;

            isUsingInternal = value;
            foreach (Interactable inter in FocusedInteractables)
            {
                // Ignore if this interactable is not interactable.
                if (value == true && inter.IsInteractable == false) continue;

                inter.IsUsing = isUsingInternal;
            }
        }
    }
    private bool isUsingInternal;

    public bool IsHovering
    {
        get => isHoveringInternal;
        private set
        {
            if (value == isHoveringInternal) return;

            isHoveringInternal = value;
            EmitSignalIsHoveringChanged(value);
        }
    }
    private bool isHoveringInternal;

    public GodotObject FocusedObject { get; private set; }

    public List<Interactable> FocusedInteractables { get; private set; } = [];

    //
    //  Private Data
    //

    private readonly List<Interactable> findInteractableCache = [];

    //
    //  Godot Methods
    //

    public override void _PhysicsProcess(double delta)
    {
        GodotObject newFocus = GetCollider();

        // If we are focusing a new object, UPDATE OUR STATE.
        if (FocusedObject != newFocus)
        {
            SetNewFocusedObject(newFocus);
        }

        // Check if the hover state has changed
        bool newHoverState = AreAnyActuallyInteractable(FocusedInteractables);
        if (newHoverState != IsHovering) IsHovering = newHoverState;
    }

    public override void _Notification(int what)
    {
        // Clean up if we are being deleted.
        if (what == NotificationPredelete) SetNewFocusedObject(null);
    }

    //
    //  Private Methods
    //

    private void SetNewFocusedObject(GodotObject newFocus)
    {
        // Find all old interactables, and stop any hovering / using.
        if (GodotObject.IsInstanceValid(FocusedObject))
        {
            findInteractableCache.Clear();
            FocusedObject?.FindInteractablesNonAlloc(findInteractableCache);
            foreach (Interactable oldInter in findInteractableCache)
            {
                if (oldInter.IsHovering) oldInter.IsHovering = false;
                if (oldInter.IsUsing) oldInter.IsUsing = false;
            }
        }

        // Start hovering new interactables.
        findInteractableCache.Clear();
        newFocus?.FindInteractablesNonAlloc(findInteractableCache);
        foreach (Interactable newInter in findInteractableCache)
        {
            newInter.IsHovering = true;
            newInter.IsUsing = IsUsing;
        }

        // Update the focused interactable cache
        FocusedInteractables.Clear();
        FocusedInteractables.AddRange(findInteractableCache);

        // Finally, update the state that we care about.
        FocusedObject = newFocus;
        IsHovering = AreAnyActuallyInteractable(findInteractableCache);
        findInteractableCache.Clear();
    }

    private bool AreAnyActuallyInteractable(List<Interactable> interactables)
    {
        foreach (Interactable interactable in interactables)
        {
            if (interactable.IsInteractable) return true;
        }
        return false;
    }
}
