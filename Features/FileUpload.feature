Feature: File Upload Automation
    As a user
    I want to upload files
    So that I can verify the upload functionality

    Scenario: Upload file and verify
        Given I am on the file upload page
        When I upload the file "test.txt"
        Then I should see the uploaded file name "test.txt"
        And I should see the upload success message 