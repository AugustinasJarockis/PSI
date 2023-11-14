using Microsoft.AspNetCore.Mvc.Testing;

namespace NoteaTests
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient httpClient;

        public UnitTest1()
        {
            var factory = new WebApplicationFactory<Program>();
            _factory = factory;
            httpClient = _factory.CreateClient();
        }
        [Fact]
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
        }

    }
}