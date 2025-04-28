import { type Locator } from 'playwright';
import { expect } from 'playwright/test';

export abstract class AbstractWebElementContainer {
    public constructor(
        protected readonly rootElement: Locator,
        protected readonly propertyName: string
    ) { }

    public async clickElement(): Promise<void> {
        await this.rootElement.click();
    }

    public async validateElementVisible(): Promise<void> {
        await expect(this.rootElement, {
            message: `${this.propertyName} should be visible`
        }).toBeVisible();
    }

    public async validateElementNotVisible(): Promise<void> {
        await expect(this.rootElement, {
            message: `${this.propertyName} should not be visible`
        }).not.toBeVisible();
    }

    public async validateElementDisabled(): Promise<void> {
        await expect(this.rootElement, {
            message: `${this.propertyName} should be disabled`
        }).toBeDisabled();
    }

    public async validateElementInViewport(): Promise<void> {
        await expect(this.rootElement, {
            message: `${this.propertyName} should be in viewport`
        }).toBeInViewport();
    }

    public async validateElementNotDisabled(): Promise<void> {
        await expect(this.rootElement, {
            message: `${this.propertyName} should not be disabled`
        }).not.toBeDisabled();
    }

    /**
     * Checks if the associated root element is visible in the viewport.
     *
     * This method checks that an element is not only displayed but also has a non-empty size
     * and is not set to `display: none` through itself or any ancestor. Note: This method
     * relies on the underlying web interaction library's `isVisible()` function.
     *
     * @returns {Promise<boolean>} A promise that resolves to `true` if the element is visible,
     *                             otherwise `false`.
     *
     * @example
     * const element = new WebElement(driver.findElement(By.css('#my-element')));
     * const visible = await element.isVisible();
     * console.log('Is visible:', visible); // Outputs: Is visible: true or false
     */
    public async isVisible(): Promise<boolean> {
        return await this.rootElement.isVisible();
    }

    /**
     * Verifies that the associated root element's text content matches the expected value.
     *
     * This method asserts that the text content of the root element corresponds exactly to the
     * provided `expectedValue`, which can be a direct text string or a regular expression pattern.
     * The test will fail if the actual text does not match the expected value.
     *
     * @param {string | RegExp} expectedValue - The text or pattern expected to be present in the
     *                                          element. It can be a plain string for exact matches
     *                                          or a regular expression for pattern-based matching.
     *
     * @returns {Promise<void>} A promise that resolves if the text content matches the expected
     *                          value, otherwise throws an error with a message specifying the
     *                          expected value.
     *
     * @example
     * // Assume the element is associated with a notification message in the UI.
     * const element = new WebElement(driver.findElement(By.id('notification')), 'Notification Message');
     * await element.haveText('Successfully updated!'); // Exact text match.
     * await element.haveText(/^Success/); // Regex pattern to match any string starting with "Success".
     *
     * // If the notification message text matches, the test passes, otherwise it fails.
     */
    public async haveText(
        expectedValue: string | RegExp | ReadonlyArray<string | RegExp>,
        options?: {
            ignoreCase?: boolean;
            timeout?: number;
            useInnerText?: boolean;
        }
    ): Promise<void> {
        await expect(this.rootElement, `${this.propertyName} should be "${expectedValue}"`).toHaveText(
            expectedValue,
            options
        );
    }

    /**
     * Retrieves the inner text of the associated root element.
     *
     * This method gets the text content contained within the root element, including child elements,
     * but excluding any HTML tags. It fetches the consolidated, readable text as seen by the users.
     *
     * @returns {Promise<string>} A promise that resolves with the inner text of the element.
     *
     * @example
     * // Assume the element is associated with a paragraph in the UI.
     * const element = new CustomElement(driver.findElement(By.css('.user-greeting')));
     * const text = await element.getInnerText();
     * console.log(text);  // Outputs the text content of the '.user-greeting' element.
     *
     * // Usage ideal for assertions or when the text needs to be extracted for further processing.
     */
    public async getInnerText(): Promise<string> {
        return await this.rootElement.innerText();
    }

    public async getInnerTextAsNumber(): Promise<number> {
        const text = await this.rootElement.innerText();

        return parseInt(text.replace(/[^\d]*/g, ''), 10);
    }

    public async attributeValueEqualTo(attribute: string, expectedValue: string): Promise<void> {
        await expect(this.rootElement, {
            message: `${this.propertyName} should have "${attribute}" attribute with value "${expectedValue}"`
        }).toHaveAttribute(attribute, expectedValue);
    }

    public async getAttributeValue(attribute: string): Promise<string | null> {
        return await this.rootElement.getAttribute(attribute);
    }

    public async validateElementHasClass(expected: string | RegExp): Promise<void> {
        await expect(
            this.rootElement,
            `${this.propertyName} should have class ${expected}`
        ).toHaveClass(expected);
    }
}
