using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using SendGrid;
using Services;

namespace FunctionEmail
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

        [Function("SendMail")]
        public async Task SendMail([QueueTrigger("email-queue-item", Connection = "AzureWebJobsStorage")] string myQueueItem, FunctionContext context)
        {
            var customers = await _customerService.GetCustomers();
            foreach (var customer in customers)
            {
                await _houseService.SendMail(customer);
            }
        }
    }
}
