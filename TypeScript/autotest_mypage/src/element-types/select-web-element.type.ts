import { type Locator } from 'playwright';
import { expect } from 'playwright/test';
import { WebElement } from './web-element.type';

export class SelectWebElement extends WebElement {
  public constructor(rootElement: Locator, propertyName: string) {
    super(rootElement, `${propertyName} select`);
  }

  public async select(option: string): Promise<void> {
    await this.rootElement.selectOption(option);
  }

  public async validateSelectedValue(option: string): Promise<void> {
    await expect(this.rootElement, `Select element value should be "${option}"`).toHaveValue(
      option
    );
  }

  public async getAllSelectOptionsValue(): Promise<Array<string>> {
    return (await this.getAllSelectOptions()).map((option) => option.value);
  }

  public async getAllSelectOptions(): Promise<Array<{ value: string; text: string }>> {
    return await this.rootElement.locator('option').evaluateAll((options) =>
      options.map((option) => {
        const htmlOptionElement = option as HTMLOptionElement;

        return {
          value: htmlOptionElement.value,
          text: htmlOptionElement.label ?? htmlOptionElement.text
        };
      })
    );
  }

  public async validateOptionsAreAvailable(expectedValues: string[]): Promise<void> {
    const actualClaimLanguages = (await this.getAllSelectOptionsValue()).filter(
      (value: string) => value !== ''
    );

    expect(actualClaimLanguages, { message: 'Expected values did not match' }).toEqual(
      expectedValues
    );
  }

  public async validateOptionsAreNotAvailable(expectedValues: string[]): Promise<void> {
    const actualClaimLanguages = (await this.getAllSelectOptionsValue()).filter(
      (value: string) => value !== ''
    );

    expect(actualClaimLanguages, { message: 'Unexpected values' }).not.toEqual(expectedValues);
  }
}
