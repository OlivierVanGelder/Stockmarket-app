describe('Loggin in', () => {
  it('fills in the username and password, then submits the form', () => {
    cy.visit('/');
    cy.get('#root .App nav ul li')
      .contains('Graphs')
      .click();

    cy.get('input#username').type('TestingRegister');
    cy.get('input#password').type('TestingRegister');

    cy.get('form').submit();
    cy.wait(200);
    cy.url().should('include', '/graphs');
  });
});
