using Microsoft.Playwright;
using System.Text.Json;

namespace MyTestAutomationFramework.API
{
    public class MockResponseHandler
    {
        private readonly IPage _page;

        public MockResponseHandler(IPage page)
        {
            _page = page;
        }

        public async Task MockApiResponseAsync<T>(string urlPattern, T responseData, int statusCode = 200)
        {
            await _page.RouteAsync(urlPattern, async route =>
            {
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = statusCode,
                    ContentType = "application/json",
                    Body = JsonSerializer.Serialize(responseData)
                });
            });
        }

        public async Task MockApiResponseFromFileAsync(string urlPattern, string jsonFilePath, int statusCode = 200)
        {
            var jsonContent = await File.ReadAllTextAsync(jsonFilePath);

            await _page.RouteAsync(urlPattern, async route =>
            {
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = statusCode,
                    ContentType = "application/json",
                    Body = jsonContent
                });
            });
        }

        public async Task MockApiFailureAsync(string urlPattern, int statusCode = 500, string errorMessage = "Internal Server Error")
        {
            await _page.RouteAsync(urlPattern, async route =>
            {
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = statusCode,
                    ContentType = "application/json",
                    Body = JsonSerializer.Serialize(new { error = errorMessage })
                });
            });
        }

        public async Task InterceptAndModifyRequestAsync(string urlPattern, Dictionary<string, string> headers)
        {
            await _page.RouteAsync(urlPattern, async route =>
            {
                var request = route.Request;
                await route.ContinueAsync(new RouteContinueOptions
                {
                    Headers = headers
                });
            });
        }

        public async Task UnrouteAllAsync()
        {
            await _page.UnrouteAllAsync();
        }
    }
}