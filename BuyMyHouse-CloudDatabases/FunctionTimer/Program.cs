using DAL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;

var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(Configure)
                .Build();

host.Run();


static void Configure(HostBuilderContext Builder, IServiceCollection Services)
{
    Services.AddSingleton<BlobStorage>();
    Services.AddSingleton<QueueStorage>();
    Services.AddSingleton<TableStorageCustomer>();
    Services.AddSingleton<TableStorageHouse>();
    Services.AddScoped<ICustomerService, CustomerService>();
    Services.AddScoped<IHouseService, HouseService>();
}
