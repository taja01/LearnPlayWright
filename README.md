# LearnPlayWright


#How to run test
npx playwright test tests/login.spec.ts // Run a single test file
npx playwright test --headed // Run tests in headed mode
npx playwright test --project=chromium // Run all the tests against a specific project
npx playwright test --workers=1 // Disable parallel execution
npx playwright test --debug // Run test in Debug mode
npx playwright test --ui // Run test in UI mode
npx playwright test --grep="@smoke" // Run tests that match RegExp