using Godot;

namespace DoveDraft;

[Tool, GlobalClass]
public partial class Billboard8Sprite3D : MeshInstance3D
{
    public override void _Process(double delta)
    {
        if (GetActiveMaterial(0) is ShaderMaterial material)
        {
            material.SetShaderParameter("y_angle", GlobalRotation.Y);
        }
    }
}
