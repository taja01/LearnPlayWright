import { test } from '@playwright/test';
import { LoginPage } from '../src/loadable-pages/login-page';

test.describe('Login functionality tests', () => {
    let loginPage: LoginPage;

    test.beforeEach(async ({ page }) => {
        // Initialize and navigate with the login page object
        if (!loginPage) {
            loginPage = new LoginPage(page);
        }
        await loginPage.goto('https://taja01.github.io/testpage/login.html');
        await loginPage.validatePageIsLoaded();

        // Clear console messages before each test to ensure clean slate
        loginPage.clearConsoleMessages();
    });

    test('Validate login page error messages', async () => {
        await loginPage.emailInput.fillInputField('guest@guest.com');
        await loginPage.passwordInput.fillInputField('Test123!');
        await loginPage.loginButton.clickElement();

        // Verify error message is visible
        await loginPage.errorMessage.validateElementVisible();
    });

    test('Validate login page error console', async () => {
        await loginPage.emailInput.fillInputField('user@user.com');
        await loginPage.passwordInput.fillInputField('user');
        await loginPage.loginButton.clickElement();

        // Validate element visibility
        await loginPage.errorMessage.validateElementNotVisible();

        // Verify the expected error message appears in console
        const expectedErrorMessage = 'Oh boy you find me.';
        loginPage.validateConsoleErrorMessageAppeared(expectedErrorMessage);
    });

    // Optional cleanup after all tests
    test.afterEach(async () => {
        loginPage.clearConsoleMessages();
    });
});