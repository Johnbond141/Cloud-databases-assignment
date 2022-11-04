using Domain.DTO;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Helpers
{
    public class HouseHelper
    {
        public static HouseDTO HouseToDTO(House houseDAL)
        {
            return new HouseDTO()
            {
                HouseId = houseDAL.HouseId,
                Title = houseDAL.Title,
                Description = houseDAL.Description,
                Mortgage = houseDAL.Mortgage,
                ImageURL = houseDAL.ImageURL,
                Street = houseDAL.Street,
                HouseNumber = houseDAL.HouseNumber,
                ZipCode = houseDAL.ZipCode
            };
        }

        public static House DTOToHouse(HouseDTO houseDTO)
        {
            return new House(houseDTO.HouseId, houseDTO.Title,houseDTO.Description, houseDTO.Mortgage, houseDTO.ImageURL,  houseDTO.Street, houseDTO.HouseNumber, houseDTO.ZipCode);
        }
    }
}
