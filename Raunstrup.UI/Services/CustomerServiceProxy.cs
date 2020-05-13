﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Raunstrup.Contract.DTOs;
using Raunstrup.Contract.Services;
using Raunstrup.UI.Models;

namespace Raunstrup.UI.Services
{
    public class CustomerServiceProxy : ICustomerService
    {
        private const string _customerRequestUri = "api/Customers";
        private IProjectService projectService;

        public CustomerServiceProxy(HttpClient client, IProjectService projectService)
        {
            this.projectService = projectService;
            Client = client;
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
                {
                    Parameters = { new NameValueHeaderValue("v", "1.0") }
                });
        }

        public HttpClient Client { get; }

        async Task ICustomerService.AddAsync(CustomerDto customer)
        {
            var json = JsonSerializer.Serialize(customer);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(_customerRequestUri, data).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        async Task<CustomerDto> ICustomerService.GetCustomerAsync(int id)
        {
            var response = await Client.GetAsync($"{_customerRequestUri}/{id}").ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return await JsonSerializer.DeserializeAsync<CustomerDto>(stream, options).ConfigureAwait(false);
        }

        async Task<IEnumerable<CustomerDto>> ICustomerService.GetCustomerAsync()
        {


            var response = await Client.GetAsync(_customerRequestUri).ConfigureAwait(false);


            response.EnsureSuccessStatusCode();




            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);



            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return await JsonSerializer.DeserializeAsync<IEnumerable<CustomerDto>>(stream, options).ConfigureAwait(false);
        }

        async Task<IEnumerable<CustomerDiscountTypeDto>> ICustomerService.GetAllCustomerDiscountType()
        {


            var response = await Client.GetAsync(_customerRequestUri + "/GetAllCustomerDiscountType").ConfigureAwait(false);


            response.EnsureSuccessStatusCode();




            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);



            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return await JsonSerializer.DeserializeAsync<IEnumerable<CustomerDiscountTypeDto>>(stream, options).ConfigureAwait(false);
        }

        async Task ICustomerService.RemoveAsync(int id)
        {
            var response = await Client.DeleteAsync($"{_customerRequestUri}/{id}").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        async Task ICustomerService.UpdateAsync(int id, CustomerDto movie)
        {
            var json = JsonSerializer.Serialize(movie);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await Client.PutAsync($"{_customerRequestUri}/{id}", data).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
        async Task ICustomerService.AddAsync(int id, int projectid)
        {
            ProjectDto project = await projectService.GetProjectAsync(projectid);
            project.CustomerId = id;
            var json = JsonSerializer.Serialize(project);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await Client.PutAsync(_customerRequestUri + "/AddCustomerToProject", data).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        async Task<IEnumerable<CustomerDto>> ICustomerService.GetFilteredCustomers(string searchString)
        {


            var response = await Client.GetAsync(_customerRequestUri + $"/search/{searchString}").ConfigureAwait(false);


            response.EnsureSuccessStatusCode();




            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);



            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return await JsonSerializer.DeserializeAsync<IEnumerable<CustomerDto>>(stream, options).ConfigureAwait(false);
        }
    }
    //metode der kalder metoden fra api-controlleren som henter listen over alle customerdiscounttype 

    //i controlleren kalder jeg denne metode så jeg kan få listen ind på cecustomerviewmodellen
}

