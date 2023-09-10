#if TOOLS
using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace GdMUT;

public static class TestLoader
{
    public static List<TestFunction> SearchForAllTests()
    {
        List<TestFunction> tests = new();
        tests.Clear();
        // get all functions with MonoTestFunctionAttribute
        ReadOnlySpan<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
        {
            Assembly assembly = assemblies[assemblyIndex];
            if (assembly.FullName.StartsWith("System.") || assembly.FullName.Equals("System"))
            {
                continue;
            }
            GD.Print($"Loading tests from {assembly.FullName}");
            LoadFunctionsFromAssembly(tests, assembly);
        }

        return tests;
    }

    private static void LoadFunctionsFromAssembly(List<TestFunction> tests, Assembly assembly)
    {
        ReadOnlySpan<Type> types = assembly.GetTypes();
        for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
        {
            LoadFunctionsFromType(tests, types[typeIndex]);
        }
    }

    private static void LoadFunctionsFromType(List<TestFunction> tests, Type type)
    {
        ReadOnlySpan<MethodInfo> methods = type.GetMethods();
        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttributes(typeof(CSTestFunctionAttribute), false);

            if (attribute.Length > 0)
            {
                if (method.ReturnType != typeof(Result))
                {
                    GD.PushError(
                        $"Method {method.Name} in {method.DeclaringType} does not return Result. Skipping it..."
                    );
                    continue;
                }
                else if (!method.IsStatic)
                {
                    GD.PushError(
                        $"Method {method.Name} in {method.DeclaringType} is not static. Skipping it..."
                    );
                    continue;
                }
                tests.Add(
                    new TestFunction()
                    {
                        Name = method.Name,
                        Type = method.DeclaringType,
                        Method = method
                    }
                );
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class CSTestFunctionAttribute : Attribute
{
    public CSTestFunctionAttribute() { }
}
#endif
