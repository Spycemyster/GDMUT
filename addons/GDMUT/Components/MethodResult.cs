#if TOOLS
using Godot;
using GDMUT;
using System;
using MonoTest;

namespace GDMUT.Components;

[Tool]
public partial class MethodResult : PanelContainer
{
    [Export]
    private RichTextLabel _methodName;

    [Export]
    private RichTextLabel _result;
    private TestFunction _function;

    public override void _EnterTree()
    {
        base._EnterTree();
    }

    public void SetMethodResult(TestFunction function)
    {
        _function = function;
        _methodName.Text = function.Method.Name;
        Reset();
    }

    public void Update()
    {
        SetSuccess(_function.Success, _function.Result);
    }

    public void Reset()
    {
        _result.Text = "";
        SelfModulate = new Color(1, 1, 1);
    }

    public void SetSuccess(bool success, string result = "")
    {
        _result.Text = result;
        Modulate = success ? new Color(0, 1, 0) : new Color(1, 0, 0);
        GD.Print($"{result} {success}");
    }
}
#endif
