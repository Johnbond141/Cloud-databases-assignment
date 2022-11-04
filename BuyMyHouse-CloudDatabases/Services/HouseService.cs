using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Domain.Model;
using Domain.DTO;
using Domain.Helpers;
using DAL;

namespace Services
{
    public class HouseService : IHouseService
    {
        private TableStorageHouse db_House { get; set; }
        private BlobStorage blobStorage { get; set; }

        public HouseService(TableStorageHouse tableStorageHouse, BlobStorage blobStorage)
        {
            this.db_House = tableStorageHouse;
            this.blobStorage = blobStorage;
        }
        public async Task<HouseDTO> CreateHouse(HouseDTO houseDTO)
        {
            houseDTO.ImageURL = "Unknown-URL";
            return HouseHelper.HouseToDTO
                (await db_House.CreateEntity
                (HouseHelper.DTOToHouse(houseDTO)));
        
        }

        public async Task<List<HouseDTO>> GetHouses(double min, double max)
        {
            List<House> results = await db_House.GetHouses(min, max);
            return results.Select(x => HouseHelper.HouseToDTO(x)).ToList();
        }

        public async Task<HouseDTO> GetEntity(string pk, string rk)
        {
            House result = await db_House.GetEntityByPartitionKeyAndRowKey(pk, rk);
            if (result is null) throw new NullReferenceException("No house found.");
            return HouseHelper.HouseToDTO(result);
        }

        public async Task<bool> UpdateHouse(HouseDTO houseDTO)
        {
            House checkIfExists = await db_House.GetEntityByPartitionKeyAndRowKey(houseDTO.HouseId, houseDTO.Street + houseDTO.ZipCode);
            if (checkIfExists is null) throw new NullReferenceException($"No customer found on with {houseDTO.HouseId}.");
            houseDTO.ImageURL = checkIfExists.ImageURL;
            House houseToEdit = HouseHelper.DTOToHouse(houseDTO);
            houseToEdit.ETag = checkIfExists.ETag;
            return await db_House.UpdateEntity(houseToEdit);
        }

        public async Task<bool> UpdateHousePicture(string houseId, string street, string zipcode, Stream stream)
        {
            var imageReferenceName = $"{houseId}-{street}-{zipcode}.png";
            var result = await blobStorage.UploadImage(imageReferenceName, stream);
            var imagUrl = await blobStorage.GetImage(imageReferenceName);
            var house = await db_House.GetEntityByPartitionKeyAndRowKey(houseId, street + zipcode);

            house.ImageURL = imagUrl;
            await db_House.UpdateEntity(house);

            return result;
        }

        public async Task<bool> DeleteHouse(string pk, string rk)
        {
            return await db_House.DeleteEntity(pk, rk);
          
        }

        public async Task CalculateMortgage(CustomerDTO customerDTO)
        {
            var max = new double();
            var min = new double();
            var content = "";
            var filename = "";

            if (customerDTO.Age < 18)
            {
                content = @$"Dear {customerDTO.FirstName} {customerDTO.LastName} You are to young and therefore not allowed to receive a mortgage for any house";

                filename = $"{customerDTO.CustomerId}-{customerDTO.FirstName}-{customerDTO.LastName}-document.html";
                await blobStorage.UploadPdf(filename, new MemoryStream(Encoding.UTF8.GetBytes(content ?? "")));
            }

            var totalMortage = customerDTO.YearlyIncome * 30;
            max = CalculateMax(totalMortage, customerDTO.Debts);
            min = CalculateMin(totalMortage, customerDTO.Debts);

            var houses = await GetHouses(min, max);

            content = @$"Dear {customerDTO.FirstName} {customerDTO.LastName}, Your mortgage range is: {min} - {max}. Houses available within this range are:";

            foreach (var house in houses)
            {
                content += $"<br>{house.Title}, {house.Description}. Image of the house: <br><img src={house.ImageURL}>";
            }

            filename = $"{customerDTO.CustomerId}-{customerDTO.FirstName}-{customerDTO.LastName}-document.html";

            await blobStorage.UploadPdf(filename, new MemoryStream(Encoding.UTF8.GetBytes(content ?? "")));
        }

        public double CalculateMax(double total, double debts)
        {
            return total * 0.6 - debts * 0.95;
        }

        public double CalculateMin(double total, double debts)
        {
            return total * 0.4 - debts * 0.94;
        }

        public async Task SendMail(CustomerDTO customerDTO)
        {
            if (customerDTO.Email is null or "") return;

            var uri = await blobStorage.GetPdf($"{customerDTO.CustomerId}-{customerDTO.FirstName}-{customerDTO.LastName}-document.html");

            try
            {
                var client = new SendGridClient(Environment.GetEnvironmentVariable("SendGridClient"));
                var from = new EmailAddress(Environment.GetEnvironmentVariable("SendGridEmailAddress"), "Buy My House App");
                var subject = "Your House Offers";
                var to = new EmailAddress(customerDTO.Email, "");
                var text = "view all the houses that are available within your mortgage range";
                var html = $"<div>Dear {customerDTO.FirstName} {customerDTO.LastName}. Here are the results of your mortgage application at BuyMyHouse!<br>" +
                                    $"<p>To view your application, use this temporary link <a href={uri}>Click here.</a>.</p></div>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, text, html);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task ClearMortgageApplications() 
        {
            await blobStorage.ClearMortgageApplications();
        }

    }
}
