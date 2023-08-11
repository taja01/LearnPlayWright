import { test as setup, expect } from '@playwright/test';

setup.describe('first test suite', () =>{
  
  setup('close cookei snackbar', async ({ page }) =>{
    await page.goto("https://www.epam.com");
    await expect(page.locator('#onetrust-banner-sdk')).toBeVisible();

    await page.locator('#onetrust-accept-btn-handler').click();
    await expect(page.locator('#onetrust-banner-sdk')).toBeHidden();

    await page.context().storageState({path: 'epam_state.json'});
  })
})
