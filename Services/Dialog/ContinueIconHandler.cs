using Godot;
using System.Threading;
using Yarn.Markup;
using YarnSpinnerGodot;

public partial class ContinueIconHandler : ActionMarkupHandler
{
    [Export]
    public ProceedIcon Proceed { get; set; }



    public override YarnTask OnCharacterWillAppear(int currentCharacterIndex, MarkupParseResult line, CancellationToken cancellationToken) => YarnTask.CompletedTask;

    public override void OnLineDisplayBegin(MarkupParseResult line, RichTextLabel text)
    {
        Proceed.Visible = false;
    }

    public override void OnLineDisplayComplete()
    {
        Proceed.Visible = true;
    }

    public override void OnLineWillDismiss()
    {
        Proceed.Visible = false;
    }

    public override void OnPrepareForLine(MarkupParseResult line, RichTextLabel text)
    {
        Proceed.Visible = false;
    }
}
