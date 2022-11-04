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

namespace FunctionApp
{
    public class CustomerFunctions
    {
        public ICustomerService _customerService { get; set; }

        public CustomerFunctions(ICustomerService userService)
        {
            this._customerService = userService;
        }

        [Function(nameof(GetUserByIdAndName))]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "Id of user to get", Description = "The id of the user to get")]
        [OpenApiParameter(name: "firstname", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "firstname of user", Description = "firstname")]
        [OpenApiParameter(name: "lastname", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "lastname of user", Description = "lastname")]
        public async Task<HttpResponseData> GetUserByIdAndName([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            var user = new Customer();
            try
            {
                Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(req.Url.Query);
                string userId = query["userId"];
                string firstname = query["firstname"];
                string lastname = query["lastname"];
                user = await _customerService.GetEntity(userId, firstname + lastname);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(user);
            return response;
        }

        [Function(nameof(CreateUser))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Customer), Required = true, Description = "User object that needs to be added to the database")]
        public async Task<HttpResponseData> CreateUser([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            var userDTO = new Customer();
            try
            {
                string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
                Customer user = JsonConvert.DeserializeObject<Customer>(requestbody);
                userDTO = await _customerService.CreateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(userDTO);
            return response;
        }

        [Function(nameof(UpdateUser))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Customer), Required = true, Description = "User to update")]
        public async Task<HttpResponseData> UpdateUser([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            var userDTO = new Customer();
            try
            {
                string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
                Customer user = JsonConvert.DeserializeObject<Customer>(requestbody);
                userDTO = await _customerService.UpdateUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(userDTO);
            return response;
        }

        [Function(nameof(DeleteUser))]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "Id of user to delete", Description = "The id of the user to delete")]
        [OpenApiParameter(name: "firstname", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "firstname of user", Description = "firstname")]
        [OpenApiParameter(name: "lastname", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "lastname of user", Description = "lastname")]
        public async Task<HttpResponseData> DeleteUser([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            var result = false;
            try
            {
                Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(req.Url.Query);
                string userId = query["userId"];
                string firstname = query["firstname"];
                string lastname = query["lastname"];
                result = await _customerService.DeleteUser(userId, firstname + lastname);
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

