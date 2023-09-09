#if TOOLS
using Godot;

namespace GdMUT;

[Tool]
public partial class GDMUT : EditorPlugin
{
    private Control _dock;

    public override void _EnterTree()
    {
        base._EnterTree();
        _dock = GD.Load<PackedScene>("res://addons/GDMUT/Dock.tscn").Instantiate<Control>();
        AddControlToDock(DockSlot.RightUl, _dock);
        GD.Print("Successfully loaded GDMUT");
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RemoveControlFromDocks(_dock);
        _dock?.Free();
    }
}
#endif
