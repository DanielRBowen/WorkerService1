# WorkerService1

## To get the windows background service to work successfully:

Add ServerGarbageCollection property to the csproj file as described [here](https://docs.microsoft.com/en-us/dotnet/core/extensions/workers).

Install Microsoft.Extensions.Hosting as described [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio). 

With Microsoft.Extensions.Hosting you can call ```UseWindowsService()``` when creating the host builder or add ```services.AddSingleton<IHostLifetime, WindowsServiceLifetime>();``` to the services when creating a WebHostBuilder. 

When publishing: set deployment-mode to Self-contained and check the produce single file and enable ReadyToRun compilation boxes as described [here](https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service).

## Running as a webhost
```ServiceBase.Run``` needs to be called when running as a webhost by passing the WebHost into a WebHostService and then passing into ServiceBase.Run

See [this](https://www.stevejgordon.co.uk/running-net-core-generic-host-applications-as-a-windows-service)

At the beginning of main
```
var isService = !(Debugger.IsAttached || args.Contains("--console"));

if (isService)
{
    var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
    var pathToContentRoot = Path.GetDirectoryName(pathToExe);
    Directory.SetCurrentDirectory(pathToContentRoot);
}
```

When running the webhost
```
if (isService)
{
    var webHostService = new WebHostService(webHost.Build());
    ServiceBase.Run(new ServiceBase[] { webHostService });
}
else
{
    await webHost.Build().RunAsync();
}
```
## If adding SignalR and need to listen on localhost:

When injecting a HttpClient set the HttpClientHandler ServerCertificateCustomValidationCallback as described [here](https://stackoverflow.com/questions/52939211/the-ssl-connection-could-not-be-established) [and here](https://stackoverflow.com/questions/51642671/adding-handler-to-default-http-client-in-asp-net-core)

```
services.AddHttpClient<IHttpClientService, HttpClientService>().ConfigurePrimaryHttpMessageHandler(() =>
{
  var httpClientHandler = new HttpClientHandler
  {
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
  };

  return httpClientHandler;
});
```

Also, when creating a HubConnectionBuilder and adding the Url (.WithUrl) set the options like [so](https://stackoverflow.com/questions/61905509/signalr-the-ssl-connection-could-not-be-established) to bypass the SSL certificate.

```
  options.WebSocketConfiguration = configuration =>
  {
    configuration.RemoteCertificateValidationCallback = (message, cert, chain, errors) => { return true; };
  };

  options.HttpMessageHandlerFactory = factory =>
  {
    if (factory is HttpClientHandler clientHandler)
      // bypass SSL certificate
      clientHandler.ServerCertificateCustomValidationCallback +=
        (sender, certificate, chain, sslPolicyErrors) => { return true; };

    return factory;
  };
```
