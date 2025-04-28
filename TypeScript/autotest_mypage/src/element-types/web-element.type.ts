import { type Locator } from 'playwright';
import { AbstractWebElementContainer } from './abstract-web-element-container.type';

export class WebElement extends AbstractWebElementContainer {
    public constructor(rootElement: Locator, propertyName: string) {
        super(rootElement, propertyName);
    }
}
