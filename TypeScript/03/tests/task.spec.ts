import { test, expect } from "@playwright/test"

test.describe('First time using storageState', () =>{
    test.use({storageState: 'epam_state.json'});

    test.beforeEach(async ({page}) =>{

        await page.goto('https://www.epam.com');
    });

    test('title check', async ({ page }) =>{
        await expect(page).toHaveTitle('EPAM | Software Engineering & Product Development Services');
    })

    test('burger menu open',async ({ page }) => {
        await page.locator('button.hamburger-menu__button').click();
        await expect(page.locator('div.hamburger-menu__dropdown-section')).toBeVisible();
    })
})