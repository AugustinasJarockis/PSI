using Microsoft.AspNetCore.Mvc.Testing;
using NOTEA.Database;
using NOTEA.Extentions;
using NOTEA.Services.UserServices;
using NuGet.Protocol.Plugins;

namespace NoteaTests
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient httpClient;
        //private readonly IUserRepository _userService;

        public UnitTest1(/*IUserRepository userService*/)
        {
            var factory = new WebApplicationFactory<Program>();
            _factory = factory;
            httpClient = _factory.CreateClient();
           //_userService = userService;
        }
        /*[Fact]
        public async void TestConspectList()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/Conspect/ConspectList/Index");
            int code = (int)response.StatusCode;
            Assert.Equal(200, code);
        }

        [Theory]
        [InlineData("konspektas")]
        public async void TestConspecData(string title)
        {
            var response = await httpClient.GetAsync("/Conspect/ConspectList/Index");
            var pageContent = await response.Content.ReadAsStringAsync();
            var contentString = pageContent.ToString();
            Assert.Contains(title, contentString);  
        }*/

        [Fact]
        public async void ShouldReturnTrue_IfTheNameIsValid()
        {
            string name = "konspektas";
            bool check = name.IsValidName();
            Assert.True(check);
        }
        [Fact]
        public async void ShouldReturnFalse_IfTheNameIsNotValid()
        {
            string name = "/.konspektas";
            bool check = name.IsValidName();
            Assert.False(check);
        }
        [Fact]
        public async void ShouldReturnFalse_IfTheEmailIsNotValid()
        {
            string name = "aa";
            bool check = name.IsValidEmail();
            Assert.False(check);
        }
        [Fact]
        public async void ShouldReturnTrue_IfTheEmailIsValid()
        {
            string name = "aa@gmail.com";
            bool check = name.IsValidEmail();
            Assert.True(check);
        }
        /*[Fact]
        public async void ShouldReturnGoodId_IfTheUsernameHasThatId()
        {
            string username = "a";
            int check = _userService.GetUserId(username);
            Assert.Equal(1, check);
        }*/


    }
}