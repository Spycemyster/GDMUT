# GDMUT

A unit testing framework for C# scripts in Godot.

## How to Use
1. Press the build button before enabling the plugin. This is to allow the plugin to build the necessary files to run.
2. Ensure the plugin is enabled in the Godot editor.
3. In a script, write a test function. Test functions must be static, parameterless, be prepended with the ```[CSTestFunction]``` attribute, and return a boolean (true for success, false for fail).
    - If an exception is thrown in your function, it will be counted as a fail.
4. Open the "C# Testing" ui on the editor (it should be on the right by default).
5. Click "Load Tests". This should populate the dock with a list of all your test functions and the types that they reside in.
6. Click on "Run Tests" to run each of the tests.

## Example Test Functions
```c#
public class TestClass
{
    #region Test Functions
    #if TOOLS
    [CSTestFunction]
    public static bool TestFunctionThatShouldPass()
    {
        int x = 0;
        x *= 1;
        return x == 0;
    }

    [CSTestFunction]
    public static bool TestFunctionThatShouldFail()
    {
        int x = 0;
        x *= 1;
        return x == 1;
    }
    #endif
    #endregion
}
```
NOTE: The '``#region``' and '``#if TOOLS``' preprocessor directives are optional. Just good practice :)