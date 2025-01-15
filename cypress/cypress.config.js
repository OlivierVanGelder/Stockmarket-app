const { defineConfig } = require('cypress')

module.exports = defineConfig({
  e2e: {
    baseUrl: 'http://localhost:3000',
    "specPattern": [
      "cypress/e2e/visit-homepage.cy.js",
      "cypress/e2e/visit-loginpage.cy.js",
      "cypress/e2e/register.cy.js",
      "cypress/e2e/logging-in.cy.js",
      "cypress/e2e/trade-stock.cy.js",
      "cypress/e2e/delete-account.cy.js",
  ],
  },
})