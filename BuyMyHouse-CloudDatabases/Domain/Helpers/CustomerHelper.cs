using Domain.DTO;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Helpers
{
    public class CustomerHelper
    {
        public static CustomerDTO CustomertoDTO(Customer customer)
        {
            return new CustomerDTO()
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                YearlyIncome = customer.YearlyIncome,
                Age = customer.Age,
                Debts = customer.Debts,
            };
        }
        public static Customer DTOToCustomer(CustomerDTO customerDTO)
        {
            return new Customer(customerDTO.CustomerId,customerDTO.FirstName,customerDTO.LastName,customerDTO.Email,customerDTO.YearlyIncome,customerDTO.Age,customerDTO.Debts);
        }
    }
}
