Feature: JavaScript Alerts Automation
    As a user
    I want to handle different types of JavaScript alerts
    So that I can verify their behavior

    Background:
        Given I am on the JavaScript alerts page

    Scenario: Handle JS Alert
        When I click on the JS Alert button
        Then I should see the alert message "You successfully clicked an alert"
        And I should accept the alert

    Scenario: Handle JS Confirm
        When I click on the JS Confirm button
        Then I should see the alert message "You clicked: Ok"
        And I should accept the confirm

    Scenario: Handle JS Prompt
        When I click on the JS Prompt button
        Then I should see the alert message "Test Message"
        And I should accept the prompt
        And I should see the result containing "You entered: Test Message" 