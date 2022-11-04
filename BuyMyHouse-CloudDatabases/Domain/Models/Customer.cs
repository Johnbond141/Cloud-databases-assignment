using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Domain.Model
{
    public class Customer : TableEntity
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public double YearlyIncome { get; set; }
        public int Age { get; set; }
        public double Debts { get; set; }

        public Customer()
        {

        }

        public Customer(string customerId, string firstName, string lastName, string email, double yearlyIncome, int age, double debts)
        {
            CustomerId = customerId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            YearlyIncome = yearlyIncome;
            Age = age;
            Debts = debts;

            PartitionKey = customerId;
            RowKey = firstName + lastName;
        }
    }
}
