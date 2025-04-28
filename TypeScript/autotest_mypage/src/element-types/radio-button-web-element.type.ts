import { type Locator } from 'playwright/test';
import { WebElement } from './web-element.type';
import { AbstractWebElementContainer } from './abstract-web-element-container.type';
import { InputWebElement } from './input-web-element.type';

export class RadioButtonWebElement extends AbstractWebElementContainer {
    public readonly input: InputWebElement;
    private readonly label: WebElement;

    public constructor(rootElement: Locator, propertyName: string) {
        super(rootElement, propertyName);
        this.input = new InputWebElement(this.rootElement.locator('input'), propertyName);
        this.label = new WebElement(this.rootElement.locator('label'), `${propertyName} label`);
    }

    public async markCheckBox(): Promise<void> {
        await this.label.clickElement();
    }

    public async validateUnMarked(): Promise<void> {
        await this.input.validateFieldNotChecked();
    }

    public async validateFieldChecked(): Promise<void> {
        await this.input.validateFieldChecked();
    }

    public override async validateElementVisible(): Promise<void> {
        await Promise.all([
            this.label.validateElementVisible(),
            this.input.validateElementNotVisible()
        ]);
    }

    public override async validateElementNotVisible(): Promise<void> {
        await Promise.all([
            this.label.validateElementNotVisible(),
            this.input.validateElementNotVisible()
        ]);
    }
}
