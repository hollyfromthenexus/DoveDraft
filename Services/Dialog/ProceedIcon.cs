using Godot;
using YarnSpinnerGodot;

public partial class ProceedIcon : TextureRect
{
    /// <summary>
    /// Reference to the dialogue runner.
    /// </summary>
    [Export] DialogueRunner dialogueRunner;

    [Export]
    public float RotateAmount { get; set; } = 10;

    [Export]
    public float RotateSpeed { get; set; } = 8;

    public override void _Process(double delta)
    {
        RotationDegrees = Mathf.Sin(Time.GetTicksMsec() / 1000.0f * RotateSpeed) * RotateAmount;
    }
}
