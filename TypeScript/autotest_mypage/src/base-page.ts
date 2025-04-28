import { type Page } from '@playwright/test';
import { DataLayerEvent } from './global-config';

export abstract class BasePage {
    public constructor(protected readonly page: Page) {
        this.page = page;
    }

    public abstract validatePageIsLoaded(): Promise<void>;

    public async clearGtm(): Promise<void> {
        await this.page.evaluate(() => {
            window.dataLayer = [];
        });
    }

    public async getEventsByEventName(expectedEvent: string): Promise<DataLayerEvent[]> {
        const gtmEvents = await this.page.evaluate((): DataLayerEvent[] => window.dataLayer);

        const requiredEvents = gtmEvents.filter((event) => event.event === expectedEvent);

        return requiredEvents;
    }
}
