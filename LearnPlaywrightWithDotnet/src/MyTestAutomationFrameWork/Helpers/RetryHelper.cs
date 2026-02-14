namespace MyTestAutomationFramework.Helpers
{
    public static class RetryHelper
    {
        public static async Task<T> RetryAsync<T>(
            Func<Task<T>> action,
            int maxRetries = 3,
            int delayMs = 1000,
            bool exponentialBackoff = true)
        {
            var exceptions = new List<Exception>();

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);

                    if (attempt == maxRetries)
                    {
                        throw new AggregateException(
                            $"Failed after {maxRetries + 1} attempts",
                            exceptions);
                    }

                    var delay = exponentialBackoff
                        ? delayMs * (int)Math.Pow(2, attempt)
                        : delayMs;

                    await Task.Delay(delay);
                }
            }

            throw new InvalidOperationException("Retry logic error");
        }

        public static async Task RetryAsync(
            Func<Task> action,
            int maxRetries = 3,
            int delayMs = 1000,
            bool exponentialBackoff = true)
        {
            await RetryAsync(async () =>
            {
                await action();
                return true;
            }, maxRetries, delayMs, exponentialBackoff);
        }

        public static async Task<T> RetryOnExceptionAsync<T, TException>(
            Func<Task<T>> action,
            int maxRetries = 3,
            int delayMs = 1000)
            where TException : Exception
        {
            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await action();
                }
                catch (TException)
                {
                    if (attempt == maxRetries) throw;
                    await Task.Delay(delayMs);
                }
            }

            throw new InvalidOperationException("Retry logic error");
        }
    }
}