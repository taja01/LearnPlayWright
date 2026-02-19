Feature: Home2

Background:
    Given I am on the home page

@tag1
Scenario: five and one times
	When I click '2' times to add new item
	And I click '1' times to remove item
	Then I should have '1' items in the list	

@tag1
Scenario: ten-nine times
	When I click '3' times to add new item
	And I click '3' times to remove item
	Then I should have '0' items in the list	

@tag1
Scenario: five and two times
	When I click '4' times to add new item
	And I click '2' times to remove item
	Then I should have '2' items in the list	
