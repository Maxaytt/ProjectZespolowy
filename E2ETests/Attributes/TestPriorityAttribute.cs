namespace E2ETests.Attributes;

/// <summary>
/// Custom attribute used to assign a priority to test methods.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestPriorityAttribute(int priority) : Attribute
{
    public int Priority { get; private set; } = priority;
}