const { test: base, expect } = require('@playwright/test');
const { OtpNyeremenyPage } = require('./OtpNyeremenyPage.ts');

const test = base.extend({
  otpPage: async ({ page }, use) => {
    await use(new OtpNyeremenyPage(page));
  }
});

test.beforeEach(async ({ otpPage }) => {
  await otpPage.goto();
})

test('Mock data', async ({ otpPage }) => {
 // await otpPage.setMock('500189731');
  await otpPage.search('50', '0189731');


});