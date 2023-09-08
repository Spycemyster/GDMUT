#if TOOLS
using Godot;
using System;

namespace GDMUT;

[Tool]
public partial class GDMUT : EditorPlugin
{
    private Control dock;

    public override void _EnterTree()
    {
        base._EnterTree();
        // Initialization of the plugin goes here.
        dock = GD.Load<PackedScene>("res://addons/MonoTest/Components/Dock.tscn")
            .Instantiate<Control>();
        GD.Print("Hello World");
        AddControlToDock(DockSlot.RightUl, dock);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RemoveControlFromDocks(dock);
        dock?.Free();
    }
}
#endif
