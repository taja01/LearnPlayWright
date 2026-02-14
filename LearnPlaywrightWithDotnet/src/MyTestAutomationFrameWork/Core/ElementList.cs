using Microsoft.Playwright;
using MyTestAutomationFramework.Helpers;

namespace MyTestAutomationFrameWork.Core
{
    public class ElementList<T> where T : AbstractWebElementContainer
    {
        private readonly ILocator _rootLocator;
        private readonly Func<ILocator, T> _factory;
        private List<T>? _cache;
        private readonly string _listName;

        public ElementList(ILocator rootLocator, Func<ILocator, T> factory, string listName = "Element List")
        {
            _rootLocator = rootLocator;
            _factory = factory;
            _listName = listName;
        }

        /// <summary>
        /// Wait until at least one element appears in the list
        /// </summary>
        public async Task WaitUntilNotEmptyAsync(int? timeout = null)
        {
            TestLogger.Debug($"Waiting for {_listName} to have at least one element");
            await _rootLocator.First.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Attached,
                Timeout = timeout
            });
            TestLogger.Success($"{_listName} has elements");
        }

        /// <summary>
        /// Get the count of elements in the list
        /// </summary>
        public async Task<int> CountAsync()
        {
            if (_cache != null)
            {
                TestLogger.Debug($"{_listName} count (cached): {_cache.Count}");
                return _cache.Count;
            }

            var count = await _rootLocator.CountAsync();
            TestLogger.Debug($"{_listName} count: {count}");
            return count;
        }

        /// <summary>
        /// Get all elements as a list of T
        /// </summary>
        public async Task<List<T>> ItemsAsync()
        {
            if (_cache != null)
            {
                TestLogger.Debug($"Returning cached {_listName} ({_cache.Count} items)");
                return _cache;
            }

            try
            {
                TestLogger.Debug($"Fetching all items from {_listName}");

                // Get all locators
                var locators = await _rootLocator.AllAsync();

                // Map them to custom wrapper T
                var elements = locators.Select(locator => _factory(locator)).ToList();

                _cache = elements;

                TestLogger.Success($"Fetched {elements.Count} items from {_listName}");
                return elements;
            }
            catch (Exception ex)
            {
                TestLogger.Error($"Failed to fetch items from {_listName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get element at specific index
        /// </summary>
        public async Task<T> GetAsync(int index)
        {
            if (_cache != null)
            {
                if (index >= _cache.Count || index < 0)
                {
                    throw new IndexOutOfRangeException($"Index {index} out of bounds for {_listName} (count: {_cache.Count})");
                }
                TestLogger.Debug($"Returning cached item at index {index} from {_listName}");
                return _cache[index];
            }

            TestLogger.Debug($"Getting item at index {index} from {_listName}");
            return _factory(_rootLocator.Nth(index));
        }

        /// <summary>
        /// Get first element
        /// </summary>
        public async Task<T> FirstAsync()
        {
            TestLogger.Debug($"Getting first item from {_listName}");
            return await GetAsync(0);
        }

        /// <summary>
        /// Get last element
        /// </summary>
        public async Task<T> LastAsync()
        {
            var count = await CountAsync();
            if (count == 0)
            {
                throw new InvalidOperationException($"{_listName} is empty");
            }
            TestLogger.Debug($"Getting last item from {_listName}");
            return await GetAsync(count - 1);
        }

        /// <summary>
        /// Clear cache to force refresh on next access
        /// </summary>
        public void Refresh()
        {
            TestLogger.Debug($"Refreshing cache for {_listName}");
            _cache = null;
        }

        /// <summary>
        /// Check if list is empty
        /// </summary>
        public async Task<bool> IsEmptyAsync()
        {
            var count = await CountAsync();
            return count == 0;
        }

        /// <summary>
        /// Check if list has elements
        /// </summary>
        public async Task<bool> HasElementsAsync()
        {
            return !await IsEmptyAsync();
        }

        /// <summary>
        /// Filter elements based on a predicate
        /// </summary>
        public async Task<List<T>> FilterAsync(Func<T, Task<bool>> predicate)
        {
            var items = await ItemsAsync();
            var filtered = new List<T>();

            foreach (var item in items)
            {
                if (await predicate(item))
                {
                    filtered.Add(item);
                }
            }

            TestLogger.Debug($"Filtered {_listName}: {filtered.Count} of {items.Count} items matched");
            return filtered;
        }

        /// <summary>
        /// Find first element matching predicate
        /// </summary>
        public async Task<T?> FindAsync(Func<T, Task<bool>> predicate)
        {
            var items = await ItemsAsync();

            foreach (var item in items)
            {
                if (await predicate(item))
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Execute action on all elements
        /// </summary>
        public async Task ForEachAsync(Func<T, Task> action)
        {
            var items = await ItemsAsync();
            TestLogger.Debug($"Executing action on {items.Count} items in {_listName}");

            foreach (var item in items)
            {
                await action(item);
            }
        }

        /// <summary>
        /// Get all text contents from the list
        /// </summary>
        public async Task<List<string>> GetAllTextContentsAsync()
        {
            var items = await ItemsAsync();
            var texts = new List<string>();

            foreach (var item in items)
            {
                var text = await item.GetTextContentAsync();
                texts.Add(text);
            }

            TestLogger.Debug($"Extracted {texts.Count} text values from {_listName}");
            return texts;
        }

        /// <summary>
        /// Wait until list has specific count
        /// </summary>
        public async Task WaitForCountAsync(int expectedCount, int timeout = 30000)
        {
            TestLogger.Debug($"Waiting for {_listName} to have {expectedCount} elements");

            var startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < timeout)
            {
                Refresh(); // Clear cache
                var count = await CountAsync();

                if (count == expectedCount)
                {
                    TestLogger.Success($"{_listName} has {expectedCount} elements");
                    return;
                }

                await Task.Delay(500);
            }

            throw new TimeoutException($"{_listName} did not reach expected count of {expectedCount} within {timeout}ms");
        }

        /// <summary>
        /// Access element using indexer syntax
        /// </summary>
        public Task<T> this[int index] => GetAsync(index);
    }
}