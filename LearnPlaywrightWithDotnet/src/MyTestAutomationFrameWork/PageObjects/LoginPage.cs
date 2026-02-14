using Microsoft.Playwright;
using MyTestAutomationFramework.PageObjects;
using MyTestAutomationFrameWork.Core;
using static Microsoft.Playwright.Assertions;

public class LoginPage : BasePage
{
    // WebElements - Exposed for granular testing
    public WebElement EmailInput { get; }
    public WebElement PasswordInput { get; }
    public WebElement LoginButton { get; }
    public WebElement ErrorMessage { get; }
    public WebElement ForgotPasswordLink { get; }

    public LoginPage(IPage page) : base(page)
    {
        EmailInput = new WebElement(Page.Locator("#email"), "Email Input");
        PasswordInput = new WebElement(Page.Locator("#password"), "Password Input");
        LoginButton = new WebElement(Page.Locator("button[type='submit']"), "Login Button");
        ErrorMessage = new WebElement(Page.Locator(".error-message"), "Error Message");
        ForgotPasswordLink = new WebElement(Page.Locator("text=Forgot Password"), "Forgot Password Link");
    }

    // Business Methods - Encapsulate workflows
    public async Task LoginAsync(string email, string password)
    {
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task LoginAndWaitForDashboardAsync(string email, string password)
    {
        await LoginAsync(email, password);
        await Expect(Page).ToHaveURLAsync(new Regex(".*/dashboard"));
    }

    public async Task VerifyLoginFailedAsync(string expectedError)
    {
        await ErrorMessage.ValidateElementVisibleAsync();
        await ErrorMessage.ContainTextAsync(expectedError);
    }

    public async Task<bool> IsLoginButtonEnabledAsync()
    {
        return await LoginButton.IsEnabledAsync();
    }
}