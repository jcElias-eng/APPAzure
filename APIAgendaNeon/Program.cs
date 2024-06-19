using APIAgendaNeon.Context;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
           .AddEnvironmentVariables()
           .Build();

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddDbContext<AgendaBDContext>(options =>
        {
            options.UseNpgsql("Host=ep-late-rain-a58675ir.us-east-2.aws.neon.tech;Username=AgendaBD_owner;Password=kMdtCTNS0P7q;Database=AgendaBD;SslMode=Require");
        });
    })
    .Build();

host.Run();
