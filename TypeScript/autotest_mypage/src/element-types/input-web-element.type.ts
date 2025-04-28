import { expect, type Locator } from 'playwright/test';
import { WebElement } from './web-element.type';

export class InputWebElement extends WebElement {
    public constructor(rootElement: Locator, propertyName: string) {
        super(rootElement, `${propertyName} input field`);
    }

    public async validateInputFieldIsEmpty(): Promise<void> {
        await expect(this.rootElement, {
            message: `${this.propertyName} input field should be empty`
        }).toHaveValue('');
    }

    public async validateInputFieldIsFilled(): Promise<void> {
        await expect(this.rootElement, {
            message: `${this.propertyName} input field should be filled`
        }).not.toHaveValue('');
    }

    public async validateInputFieldFilledWith(expectedValue: string): Promise<void> {
        await expect(this.rootElement, {
            message: `${this.propertyName} input field should be filled with "${expectedValue}"`
        }).toHaveValue(expectedValue);
    }

    public async fillInputField(value: string): Promise<void> {
        await this.rootElement.fill(value);
    }

    public async pressKey(char: string): Promise<void> {
        await this.rootElement.press(char);
    }

    public async clear(): Promise<void> {
        await this.rootElement.clear();
    }

    public async getInputValue(): Promise<string> {
        return await this.rootElement.inputValue();
    }

    public async validateFieldChecked(): Promise<void> {
        await expect(this.rootElement, {
            message: `${this.propertyName} field should be filled checked`
        }).toBeChecked();
    }

    public async validateFieldNotChecked(): Promise<void> {
        await expect(this.rootElement, {
            message: `${this.propertyName} field should not be checked`
        }).not.toBeChecked();
    }
}
