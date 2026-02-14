using Microsoft.Playwright;

namespace MyTestAutomationFrameWork.Core
{
    public class WebElement : AbstractWebElementContainer
    {
        public WebElement(ILocator locator, string propertyName) :
            base(locator, propertyName)
        {

        }
    }
}
