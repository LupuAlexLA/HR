import { ErpTemplatePage } from './app.po';

describe('Erp App', function() {
  let page: ErpTemplatePage;

  beforeEach(() => {
    page = new ErpTemplatePage();
  });

  it('should display message saying app works', async () => {
    page.navigateTo();
      expect(await page.getParagraphText()).toEqual('app works!');
  });
});
