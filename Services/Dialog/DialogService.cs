using System.Threading;
using Godot;
using Yarn.Markup;
using YarnSpinnerGodot;

namespace DoveDraft;

public partial class DialogService : ActionMarkupHandler, IDialogService, ISaveLoadable
{
    //
    //  Signals
    //

    [Signal]
    public delegate void DialogStartEventHandler();

    [Signal]
    public delegate void DialogNextLineEventHandler();

    [Signal]
    public delegate void DialogHurryUpEventHandler();

    [Signal]
    public delegate void DialogLineEndEventHandler();

    [Signal]
    public delegate void DialogCompleteEventHandler();

    //
    //  Exports
    //

    [Export]
    public DialogueRunner Runner { get; set; }

    [Export]
    public SaveLoadableDialogStorage VariableStorage { get; set; }

    //
    //  Public Data
    //

    public bool IsInSequence { get; private set; }

    public bool CoolingDown => Time.GetTicksMsec() < (lastDialogCompleteTimeMs + 500);

    //
    //  Private Data
    //

    private bool isWritingLine = false;
    private ulong lastDialogCompleteTimeMs;

    //
    //  Godot Methods
    //

    public override void _EnterTree()
    {
        Runner.onDialogueStart += OnDialogStart;
        Runner.onDialogueComplete += OnDialogComplete;

        // Fetch the yarn project from DoveDraft config and use it!
        if (Services.TryGet(out IDoveDraftConfigService configService))
        {
            // TODO - do we need to set LineProvider and VariableStorage?
            Runner.SetProject(configService.Config.GameYarnProject);
        }
        else
        {
            Log.WarnFor<DialogService>(
                $"{nameof(IDoveDraftConfigService)} not found, no {nameof(YarnProject)} used..."
            );
        }

        Services.Register<IDialogService>(this);
    }

    public override void _ExitTree()
    {
        Runner.onDialogueComplete -= OnDialogComplete;
        Runner.onDialogueStart -= OnDialogStart;

        Runner.SetProject(null);

        Services.Unregister<IDialogService>();
    }

    //
    //  IDialogService Methods
    //

    /// <inheritdoc/>
    public void RequestProceed()
    {
        // If we're already writing a line, just skip to the end of that line.
        if (isWritingLine == true)
        {
            Runner.RequestHurryUpLine();
            EmitSignalDialogHurryUp();
        }
        // OTHERWISE, go to the next line.
        else
        {
            Runner.RequestNextLine();
            EmitSignalDialogNextLine();
        }
    }

    /// <inheritdoc/>
    public void StartDialog(string nodeName) => Runner.StartDialogueForget(nodeName);

    //
    //  ISaveLoadable Methods
    //

    public BaseSaveData Save()
    {
        DialogVariableSaveData variableData = VariableStorage.Save();

        return new DialogSaveData() { Variables = variableData };
    }

    public void Load(BaseSaveData data)
    {
        DialogSaveData dialogData = (DialogSaveData)data;
        VariableStorage.Load(dialogData.Variables);
    }

    //
    //  ActionMarkupHandler Methods
    //

    public override YarnTask OnCharacterWillAppear(
        int currentCharacterIndex,
        MarkupParseResult line,
        CancellationToken cancellationToken
    ) => YarnTask.CompletedTask;

    public override void OnPrepareForLine(MarkupParseResult line, RichTextLabel text)
    {
        isWritingLine = true;
    }

    public override void OnLineDisplayBegin(MarkupParseResult line, RichTextLabel text)
    {
        // no-op
    }

    public override void OnLineDisplayComplete()
    {
        isWritingLine = false;
        EmitSignalDialogLineEnd();
    }

    public override void OnLineWillDismiss()
    {
        // no-op
    }

    //
    //  DialogRunner Methods
    //

    private void OnDialogStart()
    {
        IsInSequence = true;
        EmitSignalDialogStart();
    }

    private void OnDialogComplete()
    {
        IsInSequence = false;
        lastDialogCompleteTimeMs = Time.GetTicksMsec();
        EmitSignalDialogComplete();
    }
}

public partial class Services
{
    public static IDialogService Dialog => Get<IDialogService>();
}
