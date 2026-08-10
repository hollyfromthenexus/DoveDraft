using Godot;
using YarnSpinnerGodot;

namespace DoveDraft;

[GlobalClass]
public partial class SaveLoadableDialogStorage : InMemoryVariableStorage
{
    public DialogVariableSaveData Save()
    {
        (
            System.Collections.Generic.Dictionary<string, float> floats,
            System.Collections.Generic.Dictionary<string, string> strings,
            System.Collections.Generic.Dictionary<string, bool> bools
        ) = GetAllVariables();

        return new()
        {
            Floats = floats.ToGodot(),
            Strings = strings.ToGodot(),
            Bools = bools.ToGodot(),
        };
    }

    public void Load(DialogVariableSaveData data)
    {
        SetAllVariables(data.Floats.ToCSharp(), data.Strings.ToCSharp(), data.Bools.ToCSharp());
    }
}
