using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NoteaAPI.Models.OnlineUserListModels;
using NoteaAPI.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NoteaApiIntegrationTest
{
    public class UserControllerTests :
            IClassFixture<CustomWebApplicationFactory2<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory2<Program> _factory;
        public UserControllerTests(CustomWebApplicationFactory2<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }
        [Fact]
        public async Task LogIn_ValidUser_ReturnsOkResult()
        {
            var userModel = new UserModel
            {
                Id = 55,
                Username = "Useriukas",
                Password = "Abc123123",
                Email = "user1@example.com",
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(userModel), Encoding.UTF8, "application/json");
            var requestUri = $"api/User/login";

            var response = await _client.PostAsync(requestUri, jsonContent);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var returnedUser = JsonConvert.DeserializeObject<UserModel>(responseContent);

            Assert.Equal(userModel.Id, returnedUser.Id);
        }
        [Fact]
        public async Task SignInAsync_ValidUser_ReturnsOkResult()
        {
            var validUser = new SignInUserModel { Username = "ValidUser", Password = "Abc123123", PasswordCheck = "Abc123123", Email = "user@example.com" };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(validUser), Encoding.UTF8, "application/json");
            var requestUri = $"api/User/signin";

            var response = await _client.PostAsync(requestUri, jsonContent);

            response.EnsureSuccessStatusCode();
           
        }
        [Fact]
        public async Task SignInAsync_InvalidEmail_ReturnsBadResult()
        {
            var validUser = new SignInUserModel { Username = "ValidUser", Password = "Abc123123", PasswordCheck = "Abc123123", Email = "invalid" };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(validUser), Encoding.UTF8, "application/json");
            var requestUri = $"api/User/signin";

            var response = await _client.PostAsync(requestUri, jsonContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        }
        [Fact]
        public async Task GetOnlineUserList_ReturnsOnlineUserList()
        {
            var requestUri = "api/User/users/online";

            var response = await _client.GetAsync(requestUri);

            response.EnsureSuccessStatusCode();
        }
        [Fact]
        public async Task UpdateUser_ValidData_ReturnsUpdatedUser()
        {
            var userId = 1;
            var userModel = new UserModel
            {
                Id = userId,
                Username = "NewUsername",
                Email = "newemail@example.com"
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(userModel), Encoding.UTF8, "application/json");
            var requestUri = $"api/User/updateuser/{userId}";

            var response = await _client.PostAsync(requestUri, jsonContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var updatedUser = JsonConvert.DeserializeObject<UserModel>(content);

        }
        

    }
}
