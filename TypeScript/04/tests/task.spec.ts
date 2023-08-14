const { test: base, expect } = require('@playwright/test');

const test = base.extend({
  page: async ({ page }, use) => {
    await page.goto('https://www.epam.com');
    await page.locator('#onetrust-accept-btn-handler').click();
    await use(page);
  },
});

test('open epam.com', async ({ page }) => {
  await expect(page).toHaveTitle('EPAM | Software Engineering & Product Development Services')
});