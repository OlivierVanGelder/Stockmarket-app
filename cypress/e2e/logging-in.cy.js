describe('Loggin in', () => {
  it('fills in the username and password, then submits the form', () => {
    cy.visit('/login'); 

    cy.get('input#username')
      .type('test');
    cy.get('input#password')
      .type('TestingLogin');

    cy.get('form')
      .submit();

    cy.get('p', { timeout: 15000 })
      .should('contain.text', 'Login successful');
  });
});
