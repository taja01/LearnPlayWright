using Microsoft.Playwright;
using MyTestAutomationFrameWork.Core;

namespace MySUT.PageObjects
{
    public class ButtonsAccordion : AbstractWebElementContainer
    {
        private readonly string propertyName;

        public ButtonsAccordion(ILocator locator, string propertyName) : base(locator, propertyName)
        {
            this.propertyName = propertyName;
            ButtonWithError = new(RootElement.Locator("#generateErrorButton"), $"{propertyName} - Accordion Header");
            ButtonWithDelay = new(RootElement.Locator("#delayedButtonAttribute"), $"{propertyName} - Delayed Button");
            AddElementButton = new(RootElement.Locator("#extendListItemButton"), $"{propertyName} - Extend Button");
            RemoveElementButton = new(RootElement.Locator("#removeListItemButton"), $"{propertyName} - Remove Button");
            Entry = new ElementList<WebElement>(RootElement.Locator("#dynamicList li"), locator => new WebElement(locator, $"{propertyName} - Entry"), $"{propertyName} - Entries");
        }

        public WebElement ButtonWithError { get; }
        public WebElement ButtonWithDelay { get; }

        public WebElement AddElementButton { get; }
        public WebElement RemoveElementButton { get; }

        public ElementList<WebElement> Entry { get; }
    }
}
