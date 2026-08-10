using System.Collections.Generic;
using Godot;

namespace DoveDraft;

public static class InteractableExtensionMethods
{
    public static List<Interactable> FindInteractables(this GodotObject node)
    {
        List<Interactable> results = new();
        node.FindInteractablesNonAlloc(results);
        return results;
    }

    public static void FindInteractablesNonAlloc(this GodotObject node, List<Interactable> results)
    {
        if (node == null || node is not Node) return;

        foreach (Node child in (node as Node).GetChildren())
        {
            switch (child)
            {
                case Interactable childInteractable:
                    results.Add(childInteractable);
                    break;
            }
        }
    }
}
