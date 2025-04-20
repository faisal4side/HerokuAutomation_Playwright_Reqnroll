Feature: Login Functionality
    As a user
    I want to be able to login to the application
    So that I can access secure content

Scenario: Successful Login with Valid Credentials
    Given I am on the login page
    When I enter valid username "tomsmith" and password "SuperSecretPassword!"
    Then I should be logged in successfully

Scenario: Failed Login with Invalid Credentials
    Given I am on the login page
    When I enter invalid username "invalid" and password "invalid"
    Then I should see an error message

#Scenario: Verify Artifact Recording
#    Given I am on the login page
#    When I enter invalid username "testuser" and password "wrongpass"
#    Then I should see an error message
#    And a screenshot should be captured
#    And a video should be recorded
#    And logs should be generated
#    And stack trace should be captured 