Feature: Login Automation
    As a user
    I want to log in to the application
    So that I can access secure content

    Background:
        Given I am on the login page

        @functional @p1 @login
    Scenario: Successful Login with Valid Credentials
        When I enter valid username "tomsmith" and password "SuperSecretPassword!"
        Then I should be logged in successfully

        @functional @p1 @login
    Scenario: Failed Login with Invalid Credentials
        When I enter invalid username "invalid" and password "invalid"
        Then I should see an error message

#Scenario: Verify Artifact Recording
#    When I enter invalid username "testuser" and password "wrongpass"
#    Then I should see an error message
#    And a screenshot should be captured
#    And a video should be recorded
#    And logs should be generated
#    And stack trace should be captured 