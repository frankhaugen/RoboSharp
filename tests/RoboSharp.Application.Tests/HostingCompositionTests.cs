using Microsoft.Extensions.DependencyInjection;
using RoboSharp.Application;
using RoboSharp.Hosting;
using RoboSharp.Workspaces;

namespace RoboSharp.Application.Tests;

public class HostingCompositionTests
{
    [Test]
    public async Task AddRoboSharpHosting_Registers_Workspace_And_Execution_Services()
    {
        var services = new ServiceCollection();
        services.AddRoboSharpHosting();
        using var provider = services.BuildServiceProvider();

        await Assert.That(provider.GetRequiredService<IWorkspaceLoader>()).IsNotNull();
        await Assert.That(provider.GetRequiredService<IRoboSharpExecutionService>()).IsNotNull();
    }
}
