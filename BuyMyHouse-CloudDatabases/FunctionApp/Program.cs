using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using Services;

namespace FunctionApp
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(Configure)
                .Build();

            host.Run();
        }

        static void Configure(HostBuilderContext Builder, IServiceCollection Services)
        {
            Services.AddSingleton<BlobStorage>();
            Services.AddSingleton<QueueStorage>();
            Services.AddSingleton<TableStorageCustomer>();
            Services.AddSingleton<TableStorageHouse>();
            Services.AddScoped<ICustomerService, CustomerService>();
            Services.AddScoped<IHouseService, HouseService>();
        }
    }
}
