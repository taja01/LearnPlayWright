using Microsoft.Playwright;

namespace MyTestAutomationFramework.Helpers
{
    public static class WaitHelper
    {
        public static async Task WaitForConditionAsync(
            Func<Task<bool>> condition,
            int timeoutMs = 30000,
            int pollingIntervalMs = 500,
            string? errorMessage = null)
        {
            var startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                if (await condition())
                    return;

                await Task.Delay(pollingIntervalMs);
            }

            throw new TimeoutException(
                errorMessage ?? $"Condition not met within {timeoutMs}ms");
        }

        public static async Task<T> WaitForValueAsync<T>(
            Func<Task<T>> valueGetter,
            Func<T, bool> predicate,
            int timeoutMs = 30000,
            int pollingIntervalMs = 500)
        {
            var startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
            {
                var value = await valueGetter();
                if (predicate(value))
                    return value;

                await Task.Delay(pollingIntervalMs);
            }

            throw new TimeoutException($"Value condition not met within {timeoutMs}ms");
        }

        public static async Task WaitForElementToDisappearAsync(
            IPage page,
            string selector,
            int timeoutMs = 30000)
        {
            await WaitForConditionAsync(
                async () => !await page.Locator(selector).IsVisibleAsync(),
                timeoutMs,
                errorMessage: $"Element '{selector}' did not disappear within {timeoutMs}ms");
        }

        public static async Task WaitForUrlContainsAsync(
            IPage page,
            string urlPart,
            int timeoutMs = 30000)
        {
            await WaitForConditionAsync(
                async () => page.Url.Contains(urlPart),
                timeoutMs,
                errorMessage: $"URL did not contain '{urlPart}' within {timeoutMs}ms");
        }

        public static async Task WaitForApiCallAsync(
            IPage page,
            string urlPattern,
            int timeoutMs = 30000)
        {
            var tcs = new TaskCompletionSource<bool>();

            page.Response += (_, response) =>
            {
                if (response.Url.Contains(urlPattern))
                    tcs.TrySetResult(true);
            };

            var timeoutTask = Task.Delay(timeoutMs);
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
                throw new TimeoutException($"API call to '{urlPattern}' not detected within {timeoutMs}ms");
        }
    }
}