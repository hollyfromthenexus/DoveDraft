using System;
using Godot;

namespace DoveDraft;

public partial class DialogUiSounds : Node
{
    [Export]
    public DialogService Dialog { get; set; }

    public AudioStreamPlayer OpenSound => GetNode<AudioStreamPlayer>("Open");
    public AudioStreamPlayer NextSound => GetNode<AudioStreamPlayer>("Next");
    public AudioStreamPlayer LineEndSound => GetNode<AudioStreamPlayer>("LineEnd");
    public AudioStreamPlayer CloseSound => GetNode<AudioStreamPlayer>("Close");

    public override void _Ready()
    {
        Dialog.DialogStart += OnDialogStart;
        Dialog.DialogNextLine += OnDialogNext;
        Dialog.DialogLineEnd += OnDialogLineEnd;
        Dialog.DialogComplete += OnDialogComplete;
    }

    private void OnDialogStart()
    {
        OpenSound.Play();
    }

    private void OnDialogNext()
    {
        NextSound.Play();
    }

    private void OnDialogLineEnd()
    {
        LineEndSound.Play();
    }

    private void OnDialogComplete()
    {
        CloseSound.Play();
    }
}
