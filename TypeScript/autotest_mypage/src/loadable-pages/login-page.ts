import { Locator, Page } from "@playwright/test";
import { BasePage } from "../base-page";
import { InputWebElement } from "../element-types/input-web-element.type";
import { WebElement } from "../element-types/web-element.type";

export class LoginPage extends BasePage {
    public readonly title: WebElement;
    public readonly emailInput: InputWebElement;
    public readonly passwordInput: InputWebElement;
    public readonly loginButton: WebElement;
    public readonly errorMessage: WebElement;
    private readonly rootElement: Locator;

    public constructor(readonly page: Page) {
        super(page);
        this.rootElement = this.page.locator('[data-testid="login-page"]');
        this.title = new WebElement(this.rootElement.locator('h1'), 'Title');
        this.emailInput = new InputWebElement(this.rootElement.locator('#email'), 'Email input field');
        this.passwordInput = new InputWebElement(this.rootElement.locator('#password'), 'Email input field');
        this.loginButton = new WebElement(this.rootElement.locator('[type="submit"]'), 'Login button');
        this.errorMessage = new WebElement(this.rootElement.locator('#error'), 'Error message');
    }

    public async validatePageIsLoaded(): Promise<void> {
        await Promise.all([
            this.title.validateElementVisible(),
            this.emailInput.validateElementVisible(),
            this.passwordInput.validateElementVisible(),
            this.loginButton.validateElementVisible(),
            this.errorMessage.validateElementNotVisible()
        ])
    }
}