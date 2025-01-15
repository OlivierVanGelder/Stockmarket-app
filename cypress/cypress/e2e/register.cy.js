describe('Registering', () => {

  it('fills in the username and password, then submits the form', () => {
    cy.visit('/');
    cy.get('#root .App nav ul li')
      .contains('Graphs')
      .click();

    cy.get('#root .App div')
      .contains('Register')
      .click();

    cy.get('input#username').type('TestingRegister');
    cy.get('input#password').type('TestingRegister');

    cy.get('form').submit();

    cy.url().should('include', '/graphs');
  });
})