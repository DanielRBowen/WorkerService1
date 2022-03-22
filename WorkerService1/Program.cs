using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using WorkerService1;

// To get the windows background service to work successfully,
// Add ServerGarbageCollection property to the csproj file as described here: https://docs.microsoft.com/en-us/dotnet/core/extensions/workers
// Install Microsoft.Extensions.Hosting as described here: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio
// With Microsoft.Extensions.Hosting you can call UseWindowsService() when creating the host builder or add services.AddSingleton<IHostLifetime, WindowsServiceLifetime>(); to the
// services when creating a WebHostBuilder.
// When publishing: set deployment-mode to Self-contained and check the produce single file and enable ReadyToRun compilation boxes
// as described here: https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service

bool isWebHost = false;

if (isWebHost)
{
    IWebHostBuilder webHost = WebHost.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            // https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Hosting.WindowsServices/src/WindowsServiceLifetime.cs
            services.AddSingleton<IHostLifetime, WindowsServiceLifetime>();
            services.AddHostedService<Worker>();
        })
        .Configure(app =>
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
            });
        });

    await webHost.Build().RunAsync();
}
else
{
    // https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Hosting.WindowsServices/src/WindowsServiceLifetimeHostBuilderExtensions.cs
    IHost host = Host.CreateDefaultBuilder(args)
        .UseWindowsService()
        .ConfigureServices(services =>
        {
            services.AddHostedService<Worker>();
        })
        .Build();

    await host.RunAsync();
}