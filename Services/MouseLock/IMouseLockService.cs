namespace DoveDraft;

public interface IMouseLockService : IService
{
    /// <summary>
    /// Gets a value indicating whether the mouse is locked or unlocked.
    /// Can be controlled via <see cref="SetLock"/> and <see cref="ClearLock"/>.
    /// </summary>
    public bool IsLocked { get; }

    /// <summary>
    /// Sets some preference for locking / unlocking the mouse.
    /// </summary>
    /// <param name="key">The key for this lock preference. Can be later cleared with <see cref="ClearLock"/>.</param>
    /// <param name="isLocked">True if you want to lock the mouse, false otherwise.</param>
    /// <param name="priority">The priority of this lock preference. Higher will overrule lower priority preferences.</param>
    public void SetLock(string key, bool isLocked, int priority);

    /// <summary>
    /// Clear some preference for locking / unlocking the mouse.
    /// </summary>
    /// <param name="key">The key for the lock preference to clear.</param>
    public void ClearLock(string key);
}
