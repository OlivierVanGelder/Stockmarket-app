describe('template spec', () => {

  it('fills in the username and password, then submits the form', () => {
    cy.visit('/');
    cy.get('#root .App nav ul li')
      .contains('Graphs')
      .click();

    cy.get('#root .App div')
      .contains('Register')
      .click();

    cy.get('input#username').type('testingRegister');
    cy.get('input#password').type('TestingLogin');

    cy.get('form').submit();

    cy.get('p').should('contain.text', 'Register successful');
  });
})