import { expect, type Locator, type Page } from '@playwright/test';

export class OtpNyeremenyPage {
    readonly page: Page;
    readonly SeriesInput: Locator;
    readonly SeriesNumberInput: Locator;
    readonly CheckButton: Locator;

    constructor(page: Page) {
        this.page = page;
        this.SeriesInput = page.locator('#car-sweepstakes-series');
        this.SeriesNumberInput = page.locator('#car-sweepstakes-number');
        this.CheckButton = page.locator('#car-sweepstakes-form-btn');
    }

    // action that may be performed on the page described in form of methods
    async search(series, number) {
        await this.SeriesInput.fill(series);
        await this.SeriesNumberInput.fill(number);

        await this.CheckButton.click();
    }

    async goto(){
        await this.page.goto('https://otpbank.hu/portal/hu/Megtakaritas/ForintBetetek/Gepkocsinyeremeny');
    }

    async setMock(number){
        await this.page.route(`https://www.otpbank.hu/apps/composite/api/carsweepstakes/check/${number}`, async(route) =>{
            const json= {"number":"500189730","sweepstakes":[]}
            await route.fulfill({json})
        })
    }

}