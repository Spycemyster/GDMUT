using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace MonoTest;

public static class TestLoader
{
    public static List<TestFunction> SearchForAllTests()
    {
        List<TestFunction> _tests = new();
        _tests.Clear();
        // get all functions with MonoTestFunctionAttribute
        ReadOnlySpan<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
        {
            Assembly assembly = assemblies[assemblyIndex];
            ReadOnlySpan<Type> types = assembly.GetTypes();
            for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
            {
                Type type = types[typeIndex];
                ReadOnlySpan<MethodInfo> methods = type.GetMethods();
                foreach (var method in methods)
                {
                    var attribute = method.GetCustomAttributes(
                        typeof(CSTestFunctionAttribute),
                        false
                    );
                    if (attribute.Length > 0)
                    {
                        GD.Print("Found Test Function");
                        _tests.Add(
                            new TestFunction()
                            {
                                Name = ((CSTestFunctionAttribute)attribute[0]).Name,
                                Type = method.DeclaringType,
                                Method = method
                            }
                        );
                    }
                }
            }
        }

        GD.Print(_tests.Count);
        return _tests;
    }
}

public class TestFunction
{
    public string Name { get; set; }
    public Type Type { get; set; }
    public MethodInfo Method { get; set; }
    public bool Success { get; set; }
    public string Result { get; set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class CSTestFunctionAttribute : Attribute
{
    public string Name { get; set; }

    public CSTestFunctionAttribute(string name = "")
    {
        Name = name;
    }
}
