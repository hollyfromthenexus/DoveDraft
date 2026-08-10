namespace DoveDraft;

public interface IDialogService : IService
{
    /// <summary>
    /// Gets or sets a value indicating whether we are currently in a dialog sequence.
    /// </summary>
    public bool IsInSequence { get; }

    /// <summary>
    /// Gets a value indicating whether the dialog system is cooling down.
    /// </summary>
    public bool CoolingDown { get; }

    /// <summary>
    /// Request that the currently displaying dialog proceeds. If text is still being typewritered, then
    /// this will request that it is hurried instead.
    /// </summary>
    public void RequestProceed();

    /// <summary>
    /// Start displaying a new dialog graph.
    /// </summary>
    /// <param name="nodeName">The name of the node to start at.</param>
    public void StartDialog(string nodeName);
}
