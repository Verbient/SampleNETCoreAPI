using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using System.Text;
using Xunit;
using Microsoft.VisualStudio.TestPlatform.TestHost;


// INTEGRATIONTEST WON'T RUN AS THE DATABASE PATH IS NOT STATIC (For the sake of this Test). THIS COULD HAVE BEEN FIXED BY EXTRA CODING EFFORTS.

namespace MyApp.IntegrationTests
{
    public class ProductControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ProductControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOkResponse()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/api/product");
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetProductsById_ReturnsOkResponse()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/api/Product/1");
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}