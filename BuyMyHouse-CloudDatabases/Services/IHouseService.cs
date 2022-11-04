using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Model;

namespace Services
{
    public interface IHouseService
    {
        Task<List<HouseDTO>> GetHouses(double min, double max);
        Task<HouseDTO> CreateHouse(HouseDTO houseDTO);
        Task<HouseDTO> GetEntity(string pk, string rk);
        Task<bool> UpdateHouse(HouseDTO houseDTO);
        Task<bool> UpdateHousePicture(string customerId, string firstName, string lastName, Stream stream);
        Task CalculateMortgage(CustomerDTO customerDTO);
        Task SendMail(CustomerDTO customerDTO);
        Task<bool> DeleteHouse(string pk, string rk);
        Task ClearMortgageApplications();
    }
}
