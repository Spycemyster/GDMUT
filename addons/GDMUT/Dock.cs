#if TOOLS

using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GdMUT.Components;

[Tool]
public partial class Dock : Control
{
    [Export]
    private Button _runTests;

    [Export]
    private Button _loadTests;

    [Export]
    private VBoxContainer _testList;

    private List<TestFunction> _tests = new();
    private readonly Dictionary<Type, List<TestFunction>> _testDictionary = new();
    private readonly Dictionary<Type, TestResult> _testResultDictionary = new();

    public override void _EnterTree()
    {
        base._EnterTree();
        _runTests.Pressed += RunTests;
        _loadTests.Pressed += LoadTests;
    }

    private void LoadTests()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        foreach (Node node in _testList.GetChildren())
        {
            node.QueueFree();
        }

        _tests?.Clear();
        _tests = TestLoader.SearchForAllTests();
        _testDictionary.Clear();
        for (int testIndex = 0; testIndex < _tests.Count; testIndex++)
        {
            TestFunction function = _tests[testIndex];
            if (_testDictionary.TryGetValue(function.Type, out List<TestFunction> testList))
            {
                testList.Add(function);
            }
            else
            {
                _testDictionary.Add(function.Type, new List<TestFunction>() { function });
            }
        }

        _testResultDictionary.Clear();
        var testResultScene = GD.Load<PackedScene>("res://addons/GDMUT/TestResult.tscn");
        foreach (Type type in _testDictionary.Keys)
        {
            var functions = _testDictionary[type];
            var testResult = testResultScene.Instantiate<TestResult>();
            testResult.SetTypeName(type.Name);
            _testList.AddChild(testResult);
            _testResultDictionary.Add(type, testResult);
            foreach (TestFunction function in functions)
            {
                testResult.AddMethodResult(function);
            }
        }
        stopwatch.Stop();
        GD.Print($"Loading tests took {stopwatch.ElapsedMilliseconds}ms");
    }

    private void RunTests()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        GD.Print("Run Tests");
        foreach (var test in _tests)
        {
            GD.Print(test.Name);
            bool isSuccess;
            try
            {
                isSuccess = (bool)test.Method.Invoke(null, null);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            if (!isSuccess)
            {
                GD.Print("\t-Test Failed");
                test.Success = false;
                test.Result = "Failed";
            }
            else
            {
                GD.Print("\t-Test Passed");
                test.Success = true;
                test.Result = "Passed";
            }
        }
        stopwatch.Stop();
        UpdateUIWithResults();
        GD.Print($"Tests took {stopwatch.ElapsedMilliseconds}ms");
    }

    private void UpdateUIWithResults()
    {
        foreach (TestResult result in _testResultDictionary.Values)
        {
            result.UpdateResult();
        }
    }
}
#endif
