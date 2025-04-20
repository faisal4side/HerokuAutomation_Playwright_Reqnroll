Feature: Table Handling Automation
    As a user
    I want to interact with dynamic tables
    So that I can extract and verify data

    Scenario: Extract and verify company names from table
        Given I am on the tables page
        When I extract all company names from the table
        Then I should see the company names printed
        And I should verify that "Jason Doe" exists in the table

    Scenario: Extract data from any table
        Given I am on the tables page
        When I extract data from the table with headers
        Then I should see the table data printed 