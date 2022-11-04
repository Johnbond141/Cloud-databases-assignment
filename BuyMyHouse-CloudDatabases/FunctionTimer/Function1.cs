using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Services;

namespace FunctionTimer
{
    public class Function1
    {
        public IHouseService _houseService { get; set; }
        public ICustomerService _customerService { get; set; }

        public Function1(ICustomerService customerService, IHouseService houseService)
        {
            this._customerService = customerService;
            this._houseService = houseService;
        }

        [Function("CalculateMortgage")]
        public async Task CalculateMortgage([TimerTrigger("59 59 23 * * *")] MyInfo myTimer, FunctionContext context)
        {
            var customers = await _customerService.GetCustomers();
            foreach (var customer in customers)
            {
                await _houseService.CalculateMortgage(customer);
            }
        }

        [Function("SendMailMessageOnQueue")]
        public async Task SendMailMessageOnQueue([TimerTrigger("00 00 09 * * *")] MyInfo myTimer, FunctionContext context)
        {
            await _customerService.SendToQueue();
        }

        [Function("ClearAllMortgageApplications")]
        public async Task ClearAllMortgageApplications([TimerTrigger("00 00 10 * * *")] MyInfo myTimer, FunctionContext context)
        {
            await _houseService.ClearMortgageApplications();
        }

    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
