using Azure.Identity;
using DataAccess;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SVGService;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddSingleton<UserInfoDBContextFactory>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddAzureClients(builder =>
        {
            builder.AddQueueServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"))
                .WithCredential(new DefaultAzureCredential())
                .ConfigureOptions(c =>
                    c.MessageEncoding = Azure.Storage.Queues.QueueMessageEncoding.Base64);
        });
        services.AddHttpClient<SVGInfo>((httpclient) =>
        {
            httpclient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SVGServiceBaseURL"));
        });

    })
    .Build();

host.Run();
