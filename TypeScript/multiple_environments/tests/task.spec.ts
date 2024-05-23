const { test: base, expect } = require('@playwright/test');
const { Wikipedia } = require('./wikipedia.ts');

const test = base.extend({
  wikipedia: async ({ page }, use) => {
    await use(new Wikipedia(page));
  }
});

test.beforeEach(async ({ wikipedia }) => {
  console.log(`https://${process.env.LanguageCode}.wiktionary.org/wiki/playwright`);
  await wikipedia.goto('https://${process.env.LanguageCode}.wiktionary.org/wiki/playwright');
  console.log(process.env.LanguageCode)
})

//to debug: npx playwright test --project=chromium --debug
test('Verify title', async ({ wikipedia }) => {
  await expect(wikipedia.title).toBeVisible()
});