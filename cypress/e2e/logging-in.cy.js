describe('Loggin in', () => {
  beforeEach(() => {
    // Mock the API response for login
    cy.intercept('POST', '/api/login', {
      statusCode: 200,
      body: { message: 'Login successful' },
    }).as('loginRequest');
  });

  it('fills in the username and password, then submits the form', () => {
    cy.visit('/login'); 

    cy.get('input#username').type('test');
    cy.get('input#password').type('TestingLogin');

    cy.get('form').submit();

    cy.wait('@loginRequest');

    cy.get('p').should('contain.text', 'Login successful');
  });
});
