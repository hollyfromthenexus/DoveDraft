namespace DoveDraft;

/// <summary>
/// A service which allows you to specify a <see cref="DoveDraftConfig"/>, specific
/// to your game.
/// </summary>
public interface IDoveDraftConfigService : IService
{
    /// <summary>
    /// Gets the DoveDraft configuration, specific to your game.
    /// </summary>
    public DoveDraftConfig Config { get; }
}
