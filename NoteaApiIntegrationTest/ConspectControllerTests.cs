using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NoteaAPI.Models.ConspectModels;
using NoteaAPI.Models.RecordModels;

namespace NoteaApiIntegrationTest
{
    public class ConspectControllerTests :
            IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        public ConspectControllerTests( CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }
        [Fact]
        public async Task CreateConspects_ReturnsOkResult()
        {
            var folderId = 1;
            var userId = 1;

            var conspectModel = new ConspectModel
            {
                Id = 1,
                Date = DateTime.Now,
                ConspectSemester = ConspectSemester.Unknown, 
                Name = "ValidName",
                ConspectText = "Sample ConspectText",
                ConspectRecords = new LinkedList<RecordModel>() 
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(conspectModel), Encoding.UTF8, "application/json");
            var requestUri = $"api/Conspect/create/{folderId}/{userId}";

            var response = await _client.PostAsync(requestUri, jsonContent);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Equal(conspectModel.Id.ToString(), responseContent);
        }
        
        [Fact]
        public async Task ShareConspect_ReturnsOkResult()
        {
            var currentUsername = "user1";
            var username = "user2";
            var conspectModel = new ConspectModel
            {
                Id = 1,
                Date = DateTime.Now,
                ConspectSemester = ConspectSemester.Unknown,
                Name = "ValidName",
                ConspectText = "Sample ConspectText",
                ConspectRecords = new LinkedList<RecordModel>()
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(conspectModel), Encoding.UTF8, "application/json");
            var requestUri = $"api/Conspect/share/{currentUsername}/{username}";

            var response = await _client.PostAsync(requestUri, jsonContent);

            response.EnsureSuccessStatusCode();

        }
        [Fact]
        public async Task AddFolder_ReturnsOkResult()
        {
            var folderId = 1;
            var userId = 1;
            var folderName = "NewFolder";
            var requestUri = $"api/Conspect/folder/add/{folderId}/{userId}/{folderName}";

            var response = await _client.GetAsync(requestUri);

            response.EnsureSuccessStatusCode();
        }
        [Fact]
        public async Task GoBack_ReturnsOkResult()
        {
            var userId = 1;
            var folderId = 1;
            var requestUri = $"api/Conspect/folder/back/{userId}/{folderId}";

            var response = await _client.GetAsync(requestUri);

            response.EnsureSuccessStatusCode();
        }
    }
}
