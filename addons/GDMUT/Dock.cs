#if TOOLS

using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GdMUT.Components;

[Tool]
public partial class Dock : Control
{
    [Export]
    private LineEdit _filter;

    [Export]
    private CheckBox _multithreadedEnabled;

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
            if (!function.Name.Contains(_filter.Text))
            {
                continue;
            }
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

    private const int NUM_THREADS = 4;

    private void RunTestsInRange(int startIndex, int endIndex)
    {
        for (int testIndex = startIndex; testIndex < endIndex; testIndex++)
        {
            var test = _tests[testIndex];
            GD.Print(test.Name);
            Result testResult;
            try
            {
                testResult = (Result)test.Method.Invoke(null, null);
            }
            catch (Exception e)
            {
                testResult = new Result(false, $"Exception thrown: {e.Message}");
            }

            test.Result = testResult;
        }
    }

    private void RunTests()
    {
        if (_tests.Count == 0)
        {
            GD.Print("No tests loaded");
            return;
        }
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        if (_multithreadedEnabled.ButtonPressed)
        {
            GD.Print("Run Tests multithreaded");
            Thread[] threads = new Thread[NUM_THREADS];
            int testsPerThread =
                _tests.Count / NUM_THREADS + (_tests.Count % NUM_THREADS > 0 ? 1 : 0);
            for (int threadIndex = 0; threadIndex < NUM_THREADS; threadIndex++)
            {
                int startIndex = threadIndex * testsPerThread;
                int endIndex = Math.Min((threadIndex + 1) * testsPerThread, _tests.Count);
                threads[threadIndex] = new Thread(() => RunTestsInRange(startIndex, endIndex));
                threads[threadIndex].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }
        else
        {
            GD.Print("Run Tests singlethreaded");
            RunTestsInRange(0, _tests.Count);
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
