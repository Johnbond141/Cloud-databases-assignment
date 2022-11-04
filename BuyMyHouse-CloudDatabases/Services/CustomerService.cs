using Domain.Model;
using Domain.DTO;
using Domain.Helpers;
using DAL;

namespace Services
{
    public class CustomerService : ICustomerService
    {
        private TableStorageCustomer db_Customer { get; set; }
        private QueueStorage _queueStorage { get; set; }
        public CustomerService()
        { }

        public CustomerService(TableStorageCustomer tableStorageCustomer, QueueStorage storage)
        {
            this.db_Customer = tableStorageCustomer;
            this._queueStorage = storage;
        }

        public async Task<CustomerDTO> CreateCustomer(CustomerDTO request)
        {
            return CustomerHelper.CustomertoDTO
                (await db_Customer.CreateEntity
                (CustomerHelper.DTOToCustomer(request)));
        }

        public async Task<CustomerDTO> GetEntity(string pk, string rk)
        {
            Customer result = await db_Customer.GetEntityByPartitionKeyAndRowKey(pk, rk);
            if (result is null) throw new NullReferenceException("No customer found.");
            return CustomerHelper.CustomertoDTO(result);
        }

        public async Task<bool> UpdateCustomer(CustomerDTO customerDTO)
        {
            Customer checkIfExists = await db_Customer.GetEntityByPartitionKeyAndRowKey(customerDTO.CustomerId, customerDTO.FirstName + customerDTO.LastName);
            if (checkIfExists is null) throw new NullReferenceException($"No customer found on with {customerDTO.CustomerId}.");
            Customer customerToEdit = CustomerHelper.DTOToCustomer(customerDTO);
            customerToEdit.ETag = checkIfExists.ETag;
            return await db_Customer.UpdateEntity(customerToEdit);
        }

        public async Task<bool> DeleteCustomer(string pk, string rk)
        {
            return await db_Customer.DeleteEntity(pk, rk);
        }

        public async Task<List<CustomerDTO>> GetCustomers()
        {
            List<Customer> result = await db_Customer.GetAllCustomers();
            return result.Select(x => CustomerHelper.CustomertoDTO(x)).ToList();
        }

        public async Task SendToQueue()
        {
            await _queueStorage.CreateQueueMessage("email-queue-item", "Calculations are done. Sending email in background...");
        }


    }
}
