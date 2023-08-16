# LearnPlayWright


#How to run test
npx playwright test tests/login.spec.ts // Run a single test file
npx playwright test --headed // Run tests in headed mode
npx playwright test --project=chromium // Run all the tests against a specific project
npx playwright test --workers=1 // Disable parallel execution
npx playwright test --debug // Run test in Debug mode
npx playwright test --ui // Run test in UI mode
npx playwright test --grep="@smoke" // Run tests that match RegExp


#Slow mode
You can use the slowMo option to slow down execution (by N milliseconds per operation) and follow along while debugging.

// Chromium, Firefox, or WebKit
await chromium.launch({ headless: false, slowMo: 100 });

#Opening the HTML Report
npx playwright test
npx playwright show-report