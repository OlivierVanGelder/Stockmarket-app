describe('Visit loginpage', () => {
  it('successfully loads', () => {
    cy.visit('/');
  });

  it('finds, clicks, and asserts URL', () => {
    cy.visit('/');
    cy.get('#root .App nav ul li')
      .contains('Graphs')
      .click();

    cy.url().should('eq', 'http://localhost/login');
  });
});