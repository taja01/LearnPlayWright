Feature: Home3

Background:
    Given I am on the home page

@tag1
Scenario: five and one times
	When I click '11' times to add new item
	And I click '11' times to remove item
	Then I should have '0' items in the list	

@tag1
Scenario: ten-nine times
	When I click '7' times to add new item
	And I click '3' times to remove item
	Then I should have '4' items in the list	

@tag1
Scenario: five and two times
	When I click '1' times to add new item
	And I click '0' times to remove item
	Then I should have '1' items in the list	
