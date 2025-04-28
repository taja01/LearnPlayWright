import { test, expect } from '@playwright/test';
import { LoginPage } from '../src/loadable-pages/login-page';

test.describe('Login functionality tests', () => {

    test.beforeEach(async ({ page }) => {
        const loginPage = new LoginPage(page);

        await loginPage.goto('https://taja01.github.io/testpage/login.html');
        await loginPage.validatePageIsLoaded();
        //await loginPage.header.changeLanguage(Language.Italian);
    })

    test('Validate login page error messages', async ({ page }) => {
        const loginPage = new LoginPage(page);

        await loginPage.emailInput.fillInputField('guest@guest.com');
        await loginPage.passwordInput.fillInputField('Test123!');
        await loginPage.loginButton.clickElement();

        await loginPage.errorMessage.validateElementVisible();
    });

    test('Validate login page error console', async ({ page }) => {
        const loginPage = new LoginPage(page);

        await loginPage.emailInput.fillInputField('user@user.com');
        await loginPage.passwordInput.fillInputField('user');



        await loginPage.loginButton.clickElement();

        await loginPage.errorMessage.validateElementNotVisible();

        // Verify the error message (adjust expected error text as needed)
        const expectedErrorMessage = 'Oh boy you find me.'; // Replace with the actual error message
        loginPage.validateConsoleErrorMessageAppeared(expectedErrorMessage);
    });
})