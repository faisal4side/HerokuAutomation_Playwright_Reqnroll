Feature: JavaScript Alerts Automation
    As a user
    I want to handle different types of JavaScript alerts
    So that I can verify their behavior

    Scenario: Handle JS Alert
        Given I am on the JavaScript alerts page
        When I click on the JS Alert button
        Then I should see the alert message "I am a JS Alert"
        And I should accept the alert

    Scenario: Handle JS Confirm
        Given I am on the JavaScript alerts page
        When I click on the JS Confirm button
        Then I should see the alert message "I am a JS Confirm"
        And I should accept the confirm

    Scenario: Handle JS Prompt
        Given I am on the JavaScript alerts page
        When I click on the JS Prompt button
        Then I should see the alert message "I am a JS prompt"
        And I should accept the prompt
        And I should see the result containing "Test Message" 