import { test, expect } from '@playwright/test';

test.describe('first test suite', () =>{
  
  test.beforeEach(async ({ page }) => {
    await page.goto('https://www.epam.com');
  })

  test('has title', async ({ page }) => {    
    // Expect a title "to contain" a substring.
    await expect(page).toHaveTitle('EPAM | Software Engineering & Product Development Services')
  });
  
  test('open hamburger menu', async ({ page }) => {  
  
    await page.locator('.hamburger-menu__button').click();
  
    await expect(page.locator('.hamburger-menu__button').getAttribute('arua-expanded')).toBeTruthy()
    await expect(page.locator('.hamburger-menu__dropdown-section')).toBeVisible()
  });
})
