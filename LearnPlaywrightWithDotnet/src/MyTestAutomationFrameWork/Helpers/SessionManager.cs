using Microsoft.Playwright;

namespace MyTestAutomationFramework.Helpers
{
    public class SessionManager
    {
        private readonly IPage _page;
        private readonly string _sessionStorageDir;

        public SessionManager(IPage page, string? sessionStorageDir = null)
        {
            _page = page;
            _sessionStorageDir = sessionStorageDir
                ?? Path.Combine(Directory.GetCurrentDirectory(), "sessions");
            Directory.CreateDirectory(_sessionStorageDir);
        }

        public async Task SaveSessionAsync(string sessionName)
        {
            var storageState = await _page.Context.StorageStateAsync();
            var sessionPath = Path.Combine(_sessionStorageDir, $"{sessionName}.json");
            await File.WriteAllTextAsync(sessionPath, storageState);
        }

        public async Task<bool> LoadSessionAsync(string sessionName)
        {
            var sessionPath = Path.Combine(_sessionStorageDir, $"{sessionName}.json");

            if (!File.Exists(sessionPath))
                return false;

            var storageState = await File.ReadAllTextAsync(sessionPath);
            // You'll need to create a new context with this storage state
            // This is typically done at context creation time
            return true;
        }

        public async Task SetLocalStorageAsync(string key, string value)
        {
            await _page.EvaluateAsync($@"
                window.localStorage.setItem('{key}', '{value}');
            ");
        }

        public async Task<string?> GetLocalStorageAsync(string key)
        {
            return await _page.EvaluateAsync<string?>($@"
                window.localStorage.getItem('{key}');
            ");
        }

        public async Task ClearLocalStorageAsync()
        {
            await _page.EvaluateAsync("window.localStorage.clear();");
        }

        public async Task SetCookieAsync(string name, string value, string? domain = null)
        {
            await _page.Context.AddCookiesAsync(new[]
            {
                new Cookie
                {
                    Name = name,
                    Value = value,
                    Domain = domain ?? new Uri(_page.Url).Host,
                    Path = "/"
                }
            });
        }

        public async Task<string?> GetCookieAsync(string name)
        {
            var cookies = await _page.Context.CookiesAsync();
            return cookies.FirstOrDefault(c => c.Name == name)?.Value;
        }

        public async Task ClearCookiesAsync()
        {
            await _page.Context.ClearCookiesAsync();
        }

        public async Task ClearAllSessionDataAsync()
        {
            await ClearCookiesAsync();
            await ClearLocalStorageAsync();
        }
    }
}