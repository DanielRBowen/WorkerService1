using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using WorkerService1;

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