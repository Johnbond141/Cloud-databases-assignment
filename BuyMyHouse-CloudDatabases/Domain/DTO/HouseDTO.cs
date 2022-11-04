using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class HouseDTO
    {
        public string HouseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Mortgage { get; set; }
        public string ImageURL { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string ZipCode { get; set; }


        public HouseDTO()
        {

        }

        public HouseDTO(string houseId, string title, string description, double mortgage, string imageURL, string street, string houseNumber, string zipCode)
        {
            HouseId = houseId;
            Title = title;
            Description = description;
            Mortgage = mortgage;
            ImageURL = imageURL;
            Street = street;
            HouseNumber = houseNumber;
            ZipCode = zipCode;
        }
    }
}
