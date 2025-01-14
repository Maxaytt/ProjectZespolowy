using E2ETests.Attributes;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace E2ETests.Services;

/// <summary>
/// Custom test case orderer that orders test cases based on their priority.
/// </summary>
public class PriorityOrderer : ITestCaseOrderer
{
    /// <summary>
    /// Orders test cases based on their priority attribute, in ascending order.
    /// </summary>
    /// <typeparam name="TTestCase">The type of test case.</typeparam>
    /// <param name="testCases">The test cases to be ordered.</param>
    /// <returns>An ordered collection of test cases.</returns>
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(
        IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
    {
        string assemblyName = typeof(TestPriorityAttribute).AssemblyQualifiedName!;
        var sortedMethods = new SortedDictionary<int, List<TTestCase>>();
        foreach (TTestCase testCase in testCases)
        {
            int priority = testCase.TestMethod.Method
                .GetCustomAttributes(assemblyName)
                .FirstOrDefault()
                ?.GetNamedArgument<int>(nameof(TestPriorityAttribute.Priority)) ?? 0;

            GetOrCreate(sortedMethods, priority).Add(testCase);
        }

        foreach (TTestCase testCase in
                 sortedMethods.Keys.SelectMany(
                     priority => sortedMethods[priority].OrderBy(
                         testCase => testCase.TestMethod.Method.Name)))
        {
            yield return testCase;
        }
    }

    /// <summary>
    /// Ensures that the dictionary contains a value for the specified key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key to look up.</param>
    /// <returns>The existing or newly created value.</returns>
    private static TValue GetOrCreate<TKey, TValue>(
        IDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : struct
        where TValue : new() =>
        dictionary.TryGetValue(key, out TValue? result)
            ? result
            : (dictionary[key] = new TValue());
}