Feature: File Upload Automation
    As a user
    I want to upload files
    So that I can verify the upload functionality

    Background:
        Given I am on the file upload page

    Scenario: Upload file and verify
        When I upload the file "test.txt"
        Then I should see the uploaded file name "test.txt"
        And I should see the upload success message 