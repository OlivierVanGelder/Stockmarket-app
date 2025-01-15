describe('Deleting account', () => {
  it('deletes the account', () => {
    cy.visit('/');
    cy.get('#root .App nav ul li')
      .contains('Graphs')
      .click();

    cy.get('input#username').type('TestingRegister');
    cy.get('input#password').type('TestingRegister');

    cy.get('form').submit();
    cy.wait(500);

    cy.get('#root .App nav ul li')
      .contains('Account')
      .click();
    cy.get('button.cypress-delete-account').click();
    cy.get('button.cypress-dialog-delete-account').click();
    cy.on('window:alert', (str) => {
      expect(str).to.equal(`Account deleted successfully.`)
    })
  });
});
