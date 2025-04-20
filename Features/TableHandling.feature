Feature: Table Handling Automation
    As a user
    I want to interact with dynamic tables
    So that I can extract and verify data

Background:
	Given I am on the tables page

Scenario: Extract and print company names
	When I extract all names from the table
	Then I should see all names printed

Scenario: Verify specific name exists
	When I extract data from the table
	Then I should verify that "Jason Doe" exists in the table

Scenario: Extract and verify all table data
	When I extract data from the table
	Then I should see the complete table data printed
	And I should verify the following data exists in the table
		| Last Name | First Name | Email            | Due     | Web Site            |
		| Doe       | Jason      | jdoe@hotmail.com | $100.00 | http://www.jdoe.com |