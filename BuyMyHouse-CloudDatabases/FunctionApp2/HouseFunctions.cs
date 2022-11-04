using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Domain.Model;
using Services;
using System.Net.Mime;
using Domain.DTO;

namespace FunctionApp
{
    public class HouseFunctions
    {
        public IHouseService _houseService { get; set; }

        public HouseFunctions(IHouseService houseService)
        {
            this._houseService = houseService;
        }

        [Function(nameof(GetHouseByIdAndName))]
        [OpenApiParameter(name: "houseId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "Id of house to get", Description = "The id of the house", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "street", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "street of the house", Description = "street", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "zipcode", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "zipcode of the house", Description = "zipcode", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> GetHouseByIdAndName([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            var house = new HouseDTO();
            try
            {
                Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(req.Url.Query);
                string houseId = query["houseId"];
                string street = query["street"];
                string zipcode = query["zipcode"];
                house = await _houseService.GetEntity(houseId, street + zipcode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(house);
            return response;
        }

        [Function(nameof(GetHouseByMortgageRange))]
        [OpenApiParameter(name: "minimum", In = ParameterLocation.Query, Required = true, Type = typeof(double), Summary = "Minimum amount of the mortgage", Description = "Minimum of the mortgage", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "maximum", In = ParameterLocation.Query, Required = true, Type = typeof(double), Summary = "Maximum amount of the mortgage", Description = "Maximum of the mortgage", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> GetHouseByMortgageRange([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            List<HouseDTO> houses = new List<HouseDTO>();
            try
            {
                Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(req.Url.Query);
                double min = Double.Parse(query["minimum"]);
                double max = Double.Parse(query["maximum"]);
                houses = await _houseService.GetHouses(min, max);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(houses);
            return response;
        }

        [Function(nameof(CreateHouse))]
        [OpenApiOperation(operationId: "CreateHouse")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(HouseDTO),
        Description = "House object that needs to be added to the database")]
        public async Task<HttpResponseData> CreateHouse([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            HouseDTO houseDTO = new HouseDTO();
            try
            {
                string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
                HouseDTO request = JsonConvert.DeserializeObject<HouseDTO>(requestbody);
                houseDTO = await _houseService.CreateHouse(request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(houseDTO);
            return response;
        }

        [Function(nameof(UpdateHouse))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(HouseDTO), Required = true, Description = "House to update")]
        public async Task<HttpResponseData> UpdateHouse([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            var houseDTO = false;
            try
            {
                string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
                HouseDTO request = JsonConvert.DeserializeObject<HouseDTO>(requestbody);
                houseDTO = await _houseService.UpdateHouse(request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(houseDTO);
            return response;
        }

        [Function(nameof(UpdatePicture))]
        [OpenApiParameter(name: "houseId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "Id of house to get", Description = "The id of the house", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "street", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "street of the house", Description = "street", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "zipcode", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "zipcode of the house", Description = "zipcode", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "image/png", bodyType: typeof(MediaTypeNames.Image), Required = true, Description = "Image to upload.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Summary = "Successful operation", Description = "Successful operation")]
        public async Task<HttpResponseData> UpdatePicture([HttpTrigger(AuthorizationLevel.Anonymous, "PUT", Route = "house/image")] HttpRequestData req, FunctionContext executionContext)
        {

            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            HouseDTO house = new HouseDTO();
            var result = false;
            try
            {
                Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(req.Url.Query);
                string houseId = query["houseId"];
                string street = query["street"];
                string zipcode = query["zipcode"];
                house = await _houseService.GetEntity(houseId, street + zipcode);
                result = await _houseService.UpdateHousePicture(houseId, street, zipcode, req.Body);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(result);
            return response;
        }

        [Function(nameof(DeleteHouse))]
        [OpenApiParameter(name: "houseId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "Id of house to get", Description = "The id of the house", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "street", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "street of the house", Description = "street", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "zipcode", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "zipcode of the house", Description = "zipcode", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> DeleteHouse([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            var result = false;
            try
            {
                Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(req.Url.Query);
                string houseId = query["houseId"];
                string street = query["street"];
                string zipcode = query["zipcode"];
                result = await _houseService.DeleteHouse(houseId, street + zipcode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(result);
            return response;
        }

    }
}

