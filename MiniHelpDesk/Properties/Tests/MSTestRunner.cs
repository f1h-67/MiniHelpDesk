using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MiniHelpDesk.Tests;

/* klasa: TestRunResult
 opis: Przechowuje liczbę wyników oraz tekstowe podsumowanie uruchomionych testów.
 pola / właściwości: Passed, int, liczba zaliczonych testów; Failed, int, liczba niezaliczonych testów; Total, int, łączna liczba testów; Details, IReadOnlyList<string>, wynik każdego testu */
public sealed class TestRunResult(int passed, int failed, IReadOnlyList<string> details)
{
    public int Passed { get; } = passed;
    public int Failed { get; } = failed;
    public int Total => Passed + Failed;
    public IReadOnlyList<string> Details { get; } = details;
}

/* klasa: MSTestRunner
 opis: Uruchamia oznaczone atrybutami MSTest metody testowe bez osobnego projektu testowego.
 pola / właściwości: brak */
public sealed class MSTestRunner
{
    /* funkcja: RunAll
     opis: Odszukuje testy MSTest w bieżącym projekcie, uruchamia je i zbiera wyniki.
     parametry: brak
     zwraca: TestRunResult, podsumowanie zaliczonych i niezaliczonych testów */
    public TestRunResult RunAll()
    {
        var details = new List<string>();
        var passed = 0;
        var failed = 0;

        var testMethods = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => type.GetCustomAttribute<TestClassAttribute>() is not null)
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(method => method.GetCustomAttribute<TestMethodAttribute>() is not null)
                .Select(method => (Type: type, Method: method)))
            .OrderBy(test => test.Type.Name, StringComparer.Ordinal)
            .ThenBy(test => test.Method.Name, StringComparer.Ordinal);

        foreach (var (type, method) in testMethods)
        {
            try
            {
                var instance = method.IsStatic ? null : Activator.CreateInstance(type);
                method.Invoke(instance, null);
                passed++;
                details.Add($"✓ {method.Name}");
            }
            catch (Exception exception)
            {
                failed++;
                var cause = exception is TargetInvocationException invocation && invocation.InnerException is Exception inner
                    ? inner
                    : exception;
                details.Add($"✗ {method.Name}: {cause.Message}");
            }
        }

        return new TestRunResult(passed, failed, details);
    }
}
