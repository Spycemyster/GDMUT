#if TOOLS
using System;
using System.Reflection;

namespace GdMUT;

public class TestFunction
{
    public string Name { get; set; }
    public Type Type { get; set; }
    public MethodInfo Method { get; set; }
    public Result Result { get; set; }
}
#endif
