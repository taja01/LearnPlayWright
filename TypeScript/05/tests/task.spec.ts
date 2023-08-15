const { test: base, expect } = require('@playwright/test');
const { EPAMPage } = require('./epampage.ts');

const test = base.extend({
  epamPage: async ({ page }, use) => {
    // create EPAM page and define as custom fixture via use function
    await use(new EPAMPage(page));
  }
});

test.beforeEach(async ({ epamPage }) => {
  await epamPage.goto();
})

test('Verify that user is to search fon EPAM main page', async ({ epamPage }) => {
  await epamPage.search('Test Automation');
 // await expect(epamPage.searchResults).toBeVisible(); list.. so it will fail..
  await expect(epamPage.searchResults).toHaveCount(10);
});