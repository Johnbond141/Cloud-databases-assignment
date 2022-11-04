using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Model;

namespace Services
{
    public interface ICustomerService
    {
        Task<CustomerDTO> CreateCustomer(CustomerDTO customerDTO);
        Task<CustomerDTO> GetEntity(string pk, string rk);
        Task<List<CustomerDTO>> GetCustomers();
        Task<bool> UpdateCustomer(CustomerDTO customerDTO);
        Task<bool> DeleteCustomer(string pk, string rk);
        Task SendToQueue();
    }
}
