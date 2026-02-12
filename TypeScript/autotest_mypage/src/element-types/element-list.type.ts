import { type Locator } from '@playwright/test';

export class ElementList<T> {
  private rootLocator: Locator;
  private factory: (locator: Locator) => T;
  private cache: T[] | null = null;

  public constructor(rootLocator: Locator, factory: (locator: Locator) => T) {
    this.rootLocator = rootLocator;
    this.factory = factory;
  }

  /**
   * Waits for at least 1 item to exist.
   */
  public async waitUntilNotEmpty(options?: { timeout?: number }): Promise<void> {
    await this.rootLocator.first().waitFor({ state: 'attached', ...options });
  }

  /**
   * Returns the number of elements.
   * Uses native Playwright count if cache is empty for performance.
   */
  public async count(): Promise<number> {
    if (this.cache) {
      return this.cache.length;
    }
    return await this.rootLocator.count();
  }

 /**
   * Returns the list of wrapped elements.
   */
  public async items(): Promise<T[]> {
    if (this.cache) {
      return this.cache;
    }

    try {
      // locator.all() resolves to an array of Locators
      const locators = await this.rootLocator.all();
      
      // Map them to your custom wrapper T
      const elements = locators.map((locator) => this.factory(locator));

      this.cache = elements;
      return elements;
    } catch (error) {
      console.error('Failed to fetch or process locators:', error);
      throw error;
    }
  }
  
   /**
   * Gets a specific item by index without fetching the whole list 
   * (unless already cached).
   */
  public async get(index: number): Promise<T> {
    if (this.cache) {
      if (index >= this.cache.length) throw new Error(`Index ${index} out of bounds`);
      return this.cache[index];
    }
    // Create a specific locator for this index (lazy loading)
    return this.factory(this.rootLocator.nth(index));
  }

 /**
   * Clears the cache. 
   * Call this if the page updates (e.g., after filtering a list or adding an item).
   */
  public refresh(): void {
    this.cache = null;
  }
}