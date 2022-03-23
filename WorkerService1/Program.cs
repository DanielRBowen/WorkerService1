using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.Diagnostics;
using System.ServiceProcess;
using WorkerService1;

// To get the windows background service to work successfully,
// Add ServerGarbageCollection property to the csproj file as described here: https://docs.microsoft.com/en-us/dotnet/core/extensions/workers
// Install Microsoft.Extensions.Hosting as described here: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio
// With Microsoft.Extensions.Hosting you can call UseWindowsService() when creating the host builder or add services.AddSingleton<IHostLifetime, WindowsServiceLifetime>(); to the
// services when creating a WebHostBuilder.
// When publishing: set deployment-mode to Self-contained and check the produce single file and enable ReadyToRun compilation boxes
// as described here: https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service

var isWebHost = true;
var serverUrl = string.Empty;

//See https://www.stevejgordon.co.uk/running-net-core-generic-host-applications-as-a-windows-service
var isService = !(Debugger.IsAttached || args.Contains("--console"));



if (isService)
{
    var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
    var pathToContentRoot = Path.GetDirectoryName(pathToExe);
    Directory.SetCurrentDirectory(pathToContentRoot);
}

if (isWebHost)
{
    IWebHostBuilder webHost = WebHost.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(configurationSource =>
        {
            configurationSource.AddJsonFile(AppDomain.CurrentDomain.BaseDirectory + "/Urls.json", optional: false, reloadOnChange: true);
            var configuration = configurationSource.Build();
            serverUrl = configuration["Kestrel:Endpoints:Http:Url"];
        })
        .UseUrls(serverUrl)
        .PreferHostingUrls(true)
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

    if (isService)
    {
        var webHostService = new WebHostService(webHost.Build());
        ServiceBase.Run(new ServiceBase[] { webHostService });
    }
    else
    {
        await webHost.Build().RunAsync();
    }
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