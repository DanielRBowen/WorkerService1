# WorkerService1

To get the windows background service to work successfully,
Add ServerGarbageCollection property to the csproj file as described [here](https://docs.microsoft.com/en-us/dotnet/core/extensions/workers).

Install Microsoft.Extensions.Hosting as described [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio). 

With Microsoft.Extensions.Hosting you can call ```UseWindowsService()``` when creating the host builder or add ```services.AddSingleton<IHostLifetime, WindowsServiceLifetime>();``` to the services when creating a WebHostBuilder. 

When publishing: set deployment-mode to Self-contained and check the produce single file and enable ReadyToRun compilation boxes as described [here](https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service).
