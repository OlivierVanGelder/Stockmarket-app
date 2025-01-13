describe('Buys and sells stock', () => {
  it('passes', () => {
    cy.visit('/');
    cy.get('#root .App nav ul li')
    .contains('Graphs')
    .click();
    
    cy.get('input#username').type('CypressTest');
    cy.get('input#password').type('CypressTest');
    cy.get('form').submit();
    cy.wait(1000);
    cy.get('select.cypress-start-time-select').select('hour');
    cy.wait(3000);
    let stockAmount;

    cy.get('h2.cypress-stockAmount')
      .invoke('text')
      .then((text) => {
        const match = text.match(/\d+/);
        stockAmount = match ? parseInt(match[0], 10) : 0;
        cy.log(`Initial Stock Amount: ${stockAmount}`);
      });

    cy.get('button.cypress-buy-button').click();
    cy.get('input.cypress-amount').type('1');
    cy.get('button.cypress-purchase').click();

    cy.wait(1000);
    cy.get('h2.cypress-stockAmount')
    .invoke('text')
    .then((updatedText) => {
      const match = updatedText.match(/\d+/);
      const updatedStock = match ? parseInt(match[0], 10) : 0;
      expect(updatedStock).to.equal(stockAmount + 1);
      cy.log(`Updated Stock Amount: ${updatedStock}`);
    });

    cy.get('button.cypress-sell-button').click();
    cy.get('button.cypress-sell').click();
    
    cy.wait(1000);
    cy.get('h2.cypress-stockAmount')
    .invoke('text')
    .then((updatedText) => {
      const match = updatedText.match(/\d+/);
      const updatedStock = match ? parseInt(match[0], 10) : 0;
      expect(updatedStock).to.equal(stockAmount);
      cy.log(`Updated Stock Amount: ${updatedStock}`);
    });
  })
})