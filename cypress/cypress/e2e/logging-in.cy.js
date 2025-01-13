import './register.cy';

describe('Loggin in', () => {
  it('fills in the username and password, then submits the form', () => {
    cy.visit('/');
    cy.get('#root .App nav ul li')
      .contains('Graphs')
      .click();

    cy.get('input#username').type('CypressTest');
    cy.get('input#password').type('CypressTest');

    cy.get('form').submit();

    cy.get('p').should('contain.text', 'Login successful');
  });
});
