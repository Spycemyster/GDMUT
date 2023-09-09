#if TOOLS
using Godot;
using System.Collections.Generic;

namespace GdMUT.Components;

[Tool]
public partial class TestResult : Control
{
    [Export]
    private RichTextLabel _typeName;

    [Export]
    private VBoxContainer _methodList;

    private List<(MethodResult, TestFunction)> _functions = new();
    private const string TYPE_NAME_FORMAT = "[b][font_size=24][center]{0}[/center][/font_size][/b]";
    private string _typeNameStr;

    public override void _EnterTree()
    {
        base._EnterTree();
        foreach (Node child in _methodList.GetChildren())
        {
            child.QueueFree();
        }
        _functions.Clear();
    }

    public void SetTypeName(string typeName)
    {
        _typeNameStr = typeName;
        _typeName.Text = string.Format(TYPE_NAME_FORMAT, typeName);
    }

    public void UpdateResult()
    {
        int numSuccess = 0;
        foreach (var (methodResult, function) in _functions)
        {
            methodResult.Update();
            numSuccess += function.Success ? 1 : 0;
        }

        _typeName.Text = string.Format(
            TYPE_NAME_FORMAT,
            _typeNameStr + $" ({numSuccess}/{_functions.Count})"
        );
        if (numSuccess == _functions.Count)
        {
            _typeName.Modulate = new Color(0, 1, 0);
        }
        else if (numSuccess == 0)
        {
            _typeName.Modulate = new Color(1, 0, 0);
        }
        else
        {
            _typeName.Modulate = new Color(1, 0.9f, 0);
        }
    }

    public void AddMethodResult(TestFunction function)
    {
        var methodResultScene = GD.Load<PackedScene>("res://addons/GDMUT/MethodResult.tscn");
        var methodResult = methodResultScene.Instantiate<MethodResult>();
        methodResult.SetMethodResult(function);
        _methodList.AddChild(methodResult);
        _functions.Add((methodResult, function));
    }
}
#endif
