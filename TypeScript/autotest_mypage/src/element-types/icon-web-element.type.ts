import { expect, type Locator } from 'playwright/test';
import { WebElement } from './web-element.type';

export class IconWebElement extends WebElement {
    public constructor(rootElement: Locator, propertyName: string) {
        super(rootElement, `${propertyName} icon`);
    }

    public async validateShape(expectedShapeVectorCode: string): Promise<void> {
        await expect(this.rootElement.locator('path'), {
            message: `Expected icon shape to be '${expectedShapeVectorCode}'}'`
        }).toHaveAttribute('d', expectedShapeVectorCode);
    }

    public async validateColor(expectedColor: string): Promise<void> {
        await this.attributeValueEqualTo('fill', expectedColor);
    }
}
