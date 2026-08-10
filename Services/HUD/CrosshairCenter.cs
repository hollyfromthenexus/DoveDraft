using Godot;

namespace DoveDraft;

public partial class CrosshairCenter : TextureRect
{
    [Export]
    public Gradient ColorAnimation { get; set; }

    [Export]
    public float Speed { get; set; } = 5;

    public override void _Process(double delta)
    {
        Modulate = ColorAnimation.Sample(Mathf.Sin(Time.GetTicksMsec() / 1000.0f * Speed) + 1);
    }
}
