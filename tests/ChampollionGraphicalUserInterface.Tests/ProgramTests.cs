using System.Reflection;

namespace ChampollionGraphicalUserInterface.Tests;

public sealed class ProgramTests
{
    [Fact]
    public void Main_entry_point_requires_sta_threading()
    {
        Type programType = typeof(App).Assembly.GetType("ChampollionGraphicalUserInterface.Program", throwOnError: true)!;
        MethodInfo mainMethod = programType.GetMethod("Main", BindingFlags.Public | BindingFlags.Static)!;

        Assert.NotNull(mainMethod.GetCustomAttribute<STAThreadAttribute>());
    }
}