Feature: SmokeTest
	In order to ensure the website is working
	As a tester
	I want to navigate through different scenarios to prove the website is working

@PRSubmission @Research @ByMake
Scenario Outline: Research by Make
	Given I am on the "Home" page
	Then the "Home" page should be displayed

	When I click "Research"
	Then the "Research" page should be displayed

	When I click "By Make"
	Then the page should scroll down to the "Make" section

	When I click on a Make '<Make>'
	Then the Url should change to '<Make>'

	When I click on a Model '<Make>' '<Model>'
	Then the Url should change to '<Make>'/'<Model>'/

	When I click on a Super Trim '<SuperTrim>'
	Then the '<Overlay>' should be visible

	When I click on a Real Trim '<RealTrim>'
	Then the Url should change to the following '<Make>'/'<Model>'/'<Year>'/'<RealTrimUrl>'/

	When I click "Pricing & Offers"
	Then the side panel should expand

	When I enter the Zip Code

	When I click "Continue to Get Pricing"
	Then the leadform overlay should be displayed
	Then the Dealer should be Autobytel Test Toyota

	When I Provide Info
	| id      | value                  |
	| fName   | Bob                    |
	| lName   | Generic                |
	| strAddr | 18872 MacArthur Blvd   |
	| HParea  | 949                    |
	| HPpre   | 225                    |
	| HPphone | 4500                   |
	| email   | testlead@autobytel.com |

	When I click "Submit and Get Pricing"
	Then the PR Number should be displayed

	Examples: 
	| Make   | Model  | SuperTrim                            | Year | RealTrim         | RealTrimUrl      | Overlay                                   |
	| Ford   | Fusion | js-disablesupertrim-fusionhybrids    | 2015 | js-trimrow-35303 | fusion-hybrid-s  | overlayjs-trimcollection-fusionhybrids    |
	| Honda  | Accord | js-disablesupertrim-accordhybridbase | 2015 | js-trimrow-35764 | hybrid-base      | overlayjs-trimcollection-accordhybridbase |
	| Toyota | Venza  | js-disablesupertrim-venzaxle         | 2015 | js-trimrow-36780 | venza-xle-v6-awd | overlayjs-trimcollection-venzaxle         |
	
@PRSubmission @Research @ByCategory
Scenario Outline: Research by Category
	Given I am on the "Home" page
	Then the "Home" page should be displayed

	When I click "Research"
	Then the "Research" page should be displayed

	When I click "By Category"
	Then the page should scroll down to the "Category" section

	When I click on Category '<Category>'
	Then the Url should change to '<Category>'

	When I click on a Super Trim '<SuperTrim>'
	Then the '<Overlay>' should be visible

	When I click on a Real Trim '<RealTrim>'
	Then the Url should change to the following '<Make>'/'<Model>'/'<Year>'/'<RealTrimUrl>'/

	When I click "Pricing & Offers"
	Then the side panel should expand

	When I enter the Zip Code

	When I click "Continue to Get Pricing"
	Then the leadform overlay should be displayed
	And the Dealer should be Autobytel Test Toyota

	When I Provide Info
	| id      | value                  |
	| fName   | Bob                    |
	| lName   | Generic                |
	| strAddr | 18872 MacArthur Blvd   |
	| HParea  | 949                    |
	| HPpre   | 225                    |
	| HPphone | 4500                   |
	| email   | testlead@autobytel.com |

	When I click "Submit and Get Pricing"
	Then the PR Number should be displayed

	Examples: 
	| Category | Make   | Model  | SuperTrim                            | Year | RealTrim         | RealTrimUrl                                          | Overlay                                   |
	| Sedan    | Honda  | Accord | js-disablesupertrim-accordhybridbase | 2015 | js-trimrow-35764 | hybrid-base                                          | overlayjs-trimcollection-accordhybridbase |
	| SUV      | Toyota | Venza  | js-disablesupertrim-venzaxle         | 2015 | js-trimrow-36780 | venza-xle-v6-awd                                     | overlayjs-trimcollection-venzaxle         |
	| Truck    | Ford   | F-150  | js-disablesupertrim-f150svtraptor    | 2014 | js-trimrow-32856 | f-150-svt-raptor-4x4-supercrew-cab-styleside-5-5-box | overlayjs-trimcollection-f150svtraptor    |

@PRSubmission @CarsForSale
Scenario Outline: Cars for Sale by Make
	Given I am on the "Home" page
	Then the "Home" page should be displayed

	When I click "Cars for Sale"
	Then the "Cars for Sale" page should be displayed
	
	When I click "By Make"
	Then the page should scroll down to the "Make" section

	When I click on a Make '<Make>'
	Then the Url should change to '<Make>'

	When I click on a Model '<Make>' '<Model>'
	Then the "Results" page should be displayed
	And the '<Overlay>' should be visible if no zip code is cached

	When I Provide Info
	 | id        | value |
	 | zip_js    | 99999 |
	 | radius_js | 25    |

	When I click "I'm done, show results"
	# Then the "blah" page should be displayed

	When I click on a Vehicle
	#Then the Url should change to '<Vehicle>'

	When I click "Pricing & Offers"
	Then the side panel should expand

	When I click "Contact the Seller"
	Then the leadform overlay should be displayed

	When I Provide Info
	 | id      | value                  |
	 | fName   | Bob                    |
	 | lName   | Generic                |
	 | strAddr | 18872 MacArthur Blvd   |
	 | HParea  | 949                    |
	 | HPpre   | 225                    |
	 | HPphone | 4500                   |
	 | email   | testlead@autobytel.com |

	When I click "Contact this Dealer"
	Then the PR Number should be displayed
	
	Examples: 
	| Make   | Model  | Overlay        |
	| Ford   | Fusion | zip_overlay_js |
	| Honda  | Accord | zip_overlay_js |
	| Toyota | Venza  | zip_overlay_js |

@PRSubmission @CarsForSale
Scenario Outline: Cars for Sale by Category
	Given I am on the "Home" page
	Then the "Home" page should be displayed

	When I click "Cars for Sale"
	Then the "Cars for Sale" page should be displayed

	When I click "By Category"
	Then the page should scroll down to the "Category" section

	When I click on Category '<Category>'
	Then the "Results" page should be displayed
	And the '<Overlay>' should be visible if no zip code is cached

	When I Provide Info
	 | id        | value |
	 | zip_js    | 99999 |
	 | radius_js | 25    |

	When I click "I'm done, show results"
	# Then the "blah" page should be displayed

	When I click on a Vehicle
	#Then the Url should change to '<Vehicle>'

	When I click "Contact This Seller"
	Then the leadform overlay should be displayed

	When I Provide Info
	 | id      | value                  |
	 | fName   | Bob                    |
	 | lName   | Generic                |
	 | strAddr | 18872 MacArthur Blvd   |
	 | HParea  | 949                    |
	 | HPpre   | 225                    |
	 | HPphone | 4500                   |
	 | email   | testlead@autobytel.com |

	When I click "Contact this Dealer"
	Then the PR Number should be displayed
	
	Examples: 
	| Category | Make   | Model  | Overlay        |
	| Sedan    | Ford   | Fusion | zip_overlay_js |
	| Sedan    | Honda  | Accord | zip_overlay_js |
	| SUV      | Toyota | Venza  | zip_overlay_js |

@Tools @CarComparisons
Scenario: Car Comparisons Choose Car
	Given I am on the "Tools" page
	Then the "Tools" page should be displayed

	When I click "Car Comparisons"
	Then the page should scroll down to the "Car Comparisons" section

	When I click "Begin Your Comparison"
	Then the "Car Comparison" page should be displayed

	When I click the "1" Select a Car button
	Then the Change Car Overlay should be visible
	
	When I select a Make/Mode/Trim
	 | id                    | value              |
	 | compareCarMakeSelect  | honda              |
	 | compareCarModelSelect | accord             |
	 | compareCarTrimSelect  | 2015 EX (CVT) Coup |

	When I click the "2" Select a Car button
	Then the Change Car Overlay should be visible

	When I select a Make/Mode/Trim
	 | id                    | value   |
	 | compareCarMakeSelect  | toyota  |
	 | compareCarModelSelect | camry   |
	 | compareCarTrimSelect  | 2015 LE |

	When I click the "Compare" button
	Then the "Car Comparison Results" page should be displayed

@Tools @CarComparisons
Scenario: Car Comparisons Popular Comparisons
	Given I am on the "Tools" page
	Then the "Tools" page should be displayed

	When I click "Car Comparisons"
	Then the page should scroll down to the "Car Comparisons" section

	# As the href is #, this might need its own class. The wait till visible is not good.
	When I click the "Popular Comparsion One" button
	Then the "Car Comparison Results" page should be displayed

@Tools @PaymentCalculators
Scenario: Car Payment Calculators
	Given I am on the "Tools" page
	Then the "Tools" page should be displayed

	When I click "Payment Calculators"
	Then the page should scroll down to the "Car Payment Calculators" section

	When I click "Car Payment Calculator"
	Then the "Car Payment Calculator" page should be displayed

	When I Provide Info
	| id                            | value |
	| PaymentEstimate_purchasePrice | 32000 |
	| PaymentEstimate_cashRebate    | 0     |
	| PaymentEstimate_tradeIn       | 0     |
	| PaymentEstimate_tradeInOwed   | 0     |
	| PaymentEstimate_downPayment   | 3000  |
	| PaymentEstimate_interestRate  | 3     |
	| PaymentEstimate_termMonths    | 48    |

	When I click "Calculate Monthly Payment"
	Then the Payment Estimate should be visible

Scenario: Buying Guides
	Given I am on the "Home" page
	Then the "Home" page should be displayed

	When I click on the first Car Buying Guide
	#Then the "Car Buying Guide" page should be displayed

	When I click through the article
	Then I should see the Next Articles page