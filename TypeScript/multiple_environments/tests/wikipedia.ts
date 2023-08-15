import { expect, type Locator, type Page } from '@playwright/test';

export class Wikipedia {
    readonly page: Page;
    readonly title: Locator;

    constructor(page: Page) {
        this.page = page;
        this.title = page.locator('#firstHeading');
    }

    async goto(url){
        await this.page.goto(url);
    }
}