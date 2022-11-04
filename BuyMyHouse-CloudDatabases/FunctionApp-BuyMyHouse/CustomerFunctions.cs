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
using Domain.DTO;

namespace FunctionApp
{
    public class CustomerFunctions
    {
        public ICustomerService _customerService { get; set; }

        public CustomerFunctions(ICustomerService customerService)
        {
            this._customerService = customerService;
        }

        [Function(nameof(GetCustomerByIdAndName))]
        [OpenApiParameter(name: "CustomerId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "Id of customer to get", Description = "The id of the customer to get")]
        [OpenApiParameter(name: "FirstName", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "firstname of customer", Description = "firstname")]
        [OpenApiParameter(name: "LastName", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "lastname of customer", Description = "lastname")]
        public async Task<HttpResponseData> GetCustomerByIdAndName([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            var customer = new CustomerDTO();
            try
            {
                Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(req.Url.Query);
                string customerid = query["customerId"];
                string firstname = query["FirstName"];
                string lastname = query["LastName"];
                customer = await _customerService.GetEntity(customerid, firstname + lastname);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(customer);
            return response;
        }

        [Function(nameof(CreateCustomer))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CustomerDTO), Required = true, Description = "Customer object that needs to be added to the database")]
        public async Task<HttpResponseData> CreateCustomer([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            CustomerDTO customerDTO = new CustomerDTO();
            try
            {
                string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
                CustomerDTO request = JsonConvert.DeserializeObject<CustomerDTO>(requestbody);
                customerDTO = await _customerService.CreateCustomer(request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(customerDTO);
            return response;
        }

        [Function(nameof(UpdateCustomer))]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CustomerDTO), Required = true, Description = "Customer to update")]
        public async Task<HttpResponseData> UpdateCustomer([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            var customerDTO = false;
            try
            {
                string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
                CustomerDTO request = JsonConvert.DeserializeObject<CustomerDTO>(requestbody);
                customerDTO = await _customerService.UpdateCustomer(request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            await response.WriteAsJsonAsync(customerDTO);
            return response;
        }

        [Function(nameof(DeleteCustomer))]
        [OpenApiParameter(name: "CustomerId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "Id of customer to delete", Description = "The id of the customer to delete")]
        [OpenApiParameter(name: "FirstName", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "firstname of customer", Description = "firstname")]
        [OpenApiParameter(name: "LastName", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "lastname of customer", Description = "lastname")]
        public async Task<HttpResponseData> DeleteCustomer([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            var result = false;
            try
            {
                Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(req.Url.Query);
                string customerid = query["customerId"];
                string firstname = query["FirstName"];
                string lastname = query["LastName"];
                result = await _customerService.DeleteCustomer(customerid, firstname + lastname);
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

