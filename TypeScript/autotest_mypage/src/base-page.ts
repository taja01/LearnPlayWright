import { expect, Page } from '@playwright/test';
import { DataLayerEvent } from './global-config';

// Abstract class for Page Objects
export abstract class BasePage {
    // Instance-level console message tracker
    private consoleMessages: string[] = [];

    protected constructor(protected readonly page: Page) {
        this.page = page;

        // Capture and filter console errors
        this.page.on('console', (message) => {
            if (message.type() === 'error') {
                this.consoleMessages.push(message.text());
            }
        });
    }

    // Abstract method to ensure Page Object classes implement specific page validations
    public abstract validatePageIsLoaded(): Promise<void>;

    /**
     * Navigate to a URL and wait for the DOM content to load.
     */
    public async goto(url: string): Promise<void> {
        await this.page.goto(url);
        await this.page.waitForLoadState('domcontentloaded');
    }

    /**
     * Clears the Google Tag Manager `dataLayer` array.
     * Resets it to an empty array to ensure predictable test behavior.
     */
    public async clearGtm(): Promise<void> {
        await this.page.evaluate(() => {
            window.dataLayer = [];
        });
    }

    /**
     * Retrieves GTM events from `dataLayer` that match the specified event name.
     * 
     * @param expectedEvent - The name of the event to filter for.
     * @returns A filtered array of `DataLayerEvent` objects.
     */
    public async getEventsByEventName(expectedEvent: string): Promise<DataLayerEvent[]> {
        const gtmEvents = await this.page.evaluate((): DataLayerEvent[] => window.dataLayer || []);
        return gtmEvents.filter((event) => event.event === expectedEvent);
    }

    /**
     * Validates that the specified error message appeared in the browser console.
     * 
     * @param expectedErrorMessage - The error message or part of it to search for.
     */
    public validateConsoleErrorMessageAppeared(expectedErrorMessage: string): void {
        const errorMessageFound = this.consoleMessages.some((msg) =>
            msg.includes(expectedErrorMessage)
        );

        expect(errorMessageFound, `Expected error message "${expectedErrorMessage}" was not found in console.`).toBe(true);
    }

    /**
     * Clears all stored console messages.
     */
    public clearConsoleMessages(): void {
        this.consoleMessages = [];
    }

    /**
     * Retrieves all captured console messages for debugging or validation purposes.
     * 
     * @returns An array of console messages.
     */
    public getConsoleMessages(): string[] {
        return [...this.consoleMessages];
    }
}