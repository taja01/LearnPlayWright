import { expect, type Locator, type Page } from '@playwright/test';

export class EPAMPage {
    readonly page: Page;
    readonly searchButton: Locator;
    readonly searchInput: Locator;
    readonly findButton: Locator;
    readonly searchResults: Locator;

    // all elements of the page described in constructor
    constructor(page: Page) {
        this.page = page;
        // search icon to initiate search
        this.searchButton = page.locator('button.header-search__button');
        // search input
        this.searchInput = page.locator('#new_form_search');
        // button "FIND"
        this.findButton = page.locator('div.search-results__action-section > button');
        // collection of search results
        this.searchResults = page.locator('article.search-results__item');
    }

    // action that may be performed on the page described in form of methods
    async search(query) {
        await this.searchButton.click();
        await this.searchInput.type(query);
        await this.findButton.click();
    }

    async goto(){
        await this.page.goto('https://www.epam.com');
    }

}