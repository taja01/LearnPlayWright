using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace MyTestAutomationFrameWork.Core
{
    public abstract class AbstractWebElementContainer
    {
        private readonly string _propertyName;
        protected readonly ILocator RootElement;

        protected AbstractWebElementContainer(ILocator locator, string propertyName)
        {
            _propertyName = propertyName;
            RootElement = locator;
        }

        // Validation Methods - Clean and Simple
        public async Task ValidateElementVisibleAsync()
        {
            await Expect(RootElement).ToBeVisibleAsync();
        }

        public async Task ValidateElementNotVisibleAsync()
        {
            await Expect(RootElement).Not.ToBeVisibleAsync();
        }

        public async Task ValidateElementDisabledAsync()
        {
            await Expect(RootElement).ToBeDisabledAsync();
        }

        public async Task ValidateElementNotDisabledAsync()
        {
            await Expect(RootElement).Not.ToBeDisabledAsync();
        }

        public async Task ValidateElementEnabledAsync()
        {
            await Expect(RootElement).ToBeEnabledAsync();
        }

        public async Task ValidateElementInViewportAsync()
        {
            await Expect(RootElement).ToBeInViewportAsync();
        }

        public async Task ValidateElementHasClassAsync(string expectedClass)
        {
            await Expect(RootElement).ToHaveClassAsync(expectedClass);
        }

        public async Task ValidateElementHasClassAsync(Regex expectedClassPattern)
        {
            await Expect(RootElement).ToHaveClassAsync(expectedClassPattern);
        }

        // Text Assertions
        public async Task HaveTextAsync(string expectedText, bool? ignoreCase = null, int? timeout = null, bool? useInnerText = null)
        {
            await Expect(RootElement).ToHaveTextAsync(expectedText, new LocatorAssertionsToHaveTextOptions
            {
                IgnoreCase = ignoreCase,
                Timeout = timeout,
                UseInnerText = useInnerText
            });
        }

        public async Task HaveTextAsync(Regex expectedPattern, int? timeout = null, bool? useInnerText = null)
        {
            await Expect(RootElement).ToHaveTextAsync(expectedPattern, new LocatorAssertionsToHaveTextOptions
            {
                Timeout = timeout,
                UseInnerText = useInnerText
            });
        }

        public async Task ContainTextAsync(string expectedText, bool? ignoreCase = null, int? timeout = null)
        {
            await Expect(RootElement).ToContainTextAsync(expectedText, new LocatorAssertionsToContainTextOptions
            {
                IgnoreCase = ignoreCase,
                Timeout = timeout
            });
        }

        // Attribute Assertions
        public async Task AttributeValueEqualToAsync(string attribute, string expectedValue)
        {
            await Expect(RootElement).ToHaveAttributeAsync(attribute, expectedValue);
        }

        public async Task AttributeValueEqualToAsync(string attribute, Regex expectedPattern)
        {
            await Expect(RootElement).ToHaveAttributeAsync(attribute, expectedPattern);
        }

        // Getter Methods
        public async Task<bool> IsVisibleAsync() => await RootElement.IsVisibleAsync();
        public async Task<bool> IsHiddenAsync() => await RootElement.IsHiddenAsync();
        public async Task<bool> IsEnabledAsync() => await RootElement.IsEnabledAsync();
        public async Task<bool> IsDisabledAsync() => await RootElement.IsDisabledAsync();
        public async Task<bool> IsEditableAsync() => await RootElement.IsEditableAsync();
        public async Task<bool> IsCheckedAsync() => await RootElement.IsCheckedAsync();
        public async Task<string> GetInnerTextAsync() => await RootElement.InnerTextAsync();
        public async Task<string> GetTextContentAsync() => await RootElement.TextContentAsync() ?? string.Empty;
        public async Task<string?> GetAttributeValueAsync(string attribute) => await RootElement.GetAttributeAsync(attribute);
        public async Task<string> GetInputValueAsync() => await RootElement.InputValueAsync();

        // Action Methods
        public async Task ClickAsync(LocatorClickOptions? options = null) => await RootElement.ClickAsync(options);
        public async Task DoubleClickAsync() => await RootElement.DblClickAsync();
        public async Task HoverAsync() => await RootElement.HoverAsync();
        public async Task FillAsync(string text) => await RootElement.FillAsync(text);
        public async Task ClearAsync() => await RootElement.ClearAsync();
        public async Task TypeAsync(string text, int? delay = null) => await RootElement.PressSequentiallyAsync(text, new LocatorPressSequentiallyOptions { Delay = delay });
        public async Task CheckAsync() => await RootElement.CheckAsync();
        public async Task UncheckAsync() => await RootElement.UncheckAsync();
        public async Task ScrollIntoViewAsync() => await RootElement.ScrollIntoViewIfNeededAsync();

        public async Task WaitForAsync(WaitForSelectorState? state = null, int? timeout = null)
        {
            await RootElement.WaitForAsync(new LocatorWaitForOptions { State = state, Timeout = timeout });
        }

        // Count & Collection Methods
        public async Task<int> CountAsync() => await RootElement.CountAsync();
        public async Task<IReadOnlyList<string>> GetAllTextContentsAsync() => await RootElement.AllTextContentsAsync();
        public ILocator Nth(int index) => RootElement.Nth(index);
        public ILocator First => RootElement.First;
        public ILocator Last => RootElement.Last;

        // Screenshot
        public async Task<byte[]> ScreenshotAsync(string? path = null) => await RootElement.ScreenshotAsync(new LocatorScreenshotOptions { Path = path });

        // Property Name - Available if needed for custom logging
        public string PropertyName => _propertyName;
    }
}