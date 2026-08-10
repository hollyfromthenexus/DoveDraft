namespace DoveDraft;

public interface ICameraService : IService
{
    public VirtualCamera3D CurrentTarget { get; }

    public bool Register(VirtualCamera3D virtualCamera);
    public bool Unregister(VirtualCamera3D virtualCamera);

    public void FlagCameraTeleported(VirtualCamera3D camera = null);
}
