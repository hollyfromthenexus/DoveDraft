namespace DoveDraft;

public interface IHUDService : IService
{
    public bool Visible { get; set; }

    public Crosshair Crosshair { get; }
}
