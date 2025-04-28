import { type Locator } from 'playwright';

export class ElementList<T> {
    private locatorsPromise: Locator;
    private factory: (locator: Locator) => T;
    private cache: T[] | null = null;

    public constructor(locatorsPromise: Locator, factory: (locator: Locator) => T) {
        this.locatorsPromise = locatorsPromise;
        this.factory = factory;
    }

    public async items(): Promise<T[]> {
        if (this.cache) {
            return this.cache;
        }

        try {
            const locators = await this.locatorsPromise.all();

            const elements = locators.map((locator) => this.factory(locator));

            this.cache = elements;

            return elements;
        } catch (error) {
            console.error('Failed to fetch or process locators:', error);
            throw error;
        }
    }
}
