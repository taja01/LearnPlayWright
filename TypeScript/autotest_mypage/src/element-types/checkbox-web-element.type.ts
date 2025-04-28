import { type Locator } from 'playwright/test';
import { AbstractWebElementContainer } from './abstract-web-element-container.type';
import { RadioButtonWebElement } from './radio-button-web-element.type';

export class CheckBoxWebElement extends AbstractWebElementContainer {
    public readonly checkBox: RadioButtonWebElement;

    public constructor(rootElement: Locator, propertyName: string) {
        super(rootElement, propertyName);
        this.checkBox = new RadioButtonWebElement(this.rootElement, propertyName);
    }
}
