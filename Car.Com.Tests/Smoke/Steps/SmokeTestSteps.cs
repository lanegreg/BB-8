using System;
using System.Linq;
using Car.Com.Common;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace Car.Com.Tests.Smoke.Steps
{
    [Binding]
    public class SmokeTestSteps
    {
        #region Configuration

        private IWebDriver _driver;
        private readonly string _browser;
        private readonly string _siteDomain;
        private string _results { get; set; }
        private string _confirmationNumber { get; set; }
        private string _driverLocation;
        private string _userAgent;
        private WebDriverWait _wait;

        #endregion Configuration

        #region Constructor

        public SmokeTestSteps()
        {
            _browser = "Chrome";
            _siteDomain = "http://w3.qa.car.com/";
        }

        #endregion Constructor

        #region Setup and TearDown

        [BeforeScenario()]
        [SetUp]
        public virtual void Setup()
        {
            _driverLocation = "C:\\Scripts\\bin";

            switch (_browser)
            {
                case "Chrome":
                    {
                        var options = new ChromeOptions();

                        if (_userAgent != null)
                        {
                            options.AddArgument("--user-agent=" + _userAgent);
                        }

                        options.AddArguments("start-maximized");
                        //options.AddArguments("incognito");
                        _driver = _driverLocation == null ? new ChromeDriver(options) : new ChromeDriver(_driverLocation, options);
                        break;
                    }
            }

            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
        }

        [AfterScenario()]
        [TearDown]
        public virtual void TearDown()
        {
            _driver.Close();
            _driver.Dispose();
        }

        #endregion Setup and TearDown

        #region Tests

            #region Given

            [Given(@"I am on the ""(.*)"" page")]
            public void GivenIAmOnThePage(string p0)
            {
                var initialUrl = "";

                if (ScenarioContext.Current.ScenarioInfo.Tags.Contains("Tools"))
                {
                    initialUrl = _siteDomain + "tools/";
                }
                else
                {
                    initialUrl = _siteDomain;
                }

                _driver.Navigate().GoToUrl(initialUrl);
                System.Threading.Thread.Sleep(1000);
            }

            #endregion Given

            #region When

            [When(@"I click ""(.*)""")]
            public void WhenIClick(string p0)
            {
                var id = "";

                switch (p0)
                {
                    case "Research":
                        id = "research_st";
                        break;
                    case "Cars for Sale":
                        id = "c4s_st";
                        break;
                    case "By Make":
                        id = "by_make_st";
                        break;
                    case "By Category":
                        id = "by_cat_st";
                        break;
                    case "Car Comparisons":
                        id = "comparisons_btn_st";
                        break;
                    case "Payment Calculators":
                        id = "calculators_btn_st";
                        break;
                    case "More Research":
                        id = "more_st";
                        break;
                    case "Pricing & Offers":
                        id = "pno_btn_js";
                        break;
                    case "Continue to Get Pricing":
                        id = "get_pricing_btn_js";
                        break;
                    case "Submit and Get Pricing":
                        id = "submit_lead_btn_js";
                        break;
                    case "I'm done, show results":
                        id = "query_btn_st";
                        break;
                    case "Contact this Dealer":
                        id = "submit_lead_btn_js";
                        break;
                    case "Contact This Seller":
                        id = "get_pricing_btn_js";
                        break;
                    case "Contact the Seller":
                        id = "get_pricing_btn_js";
                        break;
                    case "Begin Your Comparison":
                        id = "custom_comparisons_btn_st";
                        break;
                    case "Car Payment Calculator":
                        id = "payment_btn_st";
                        break;
                    case "Calculate Monthly Payment":
                        id = "btnCalculateMonthlyPayment";
                        break;
                }

                System.Threading.Thread.Sleep(1000);
                _wait.Until(ExpectedConditions.ElementIsVisible(By.Id(id)));
                _driver.FindElement(By.Id(id)).Click();
            }

            [When(@"I click the ""(.*)"" Select a Car button")]
            public void WhenIClickTheSelectACarButton(string p0)
            {
                System.Threading.Thread.Sleep(1000);
                _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("selectedCompareTable")));
                _driver.FindElement(By.Id("selectedCompareTable")).FindElement(By.CssSelector("button:nth-child(" + p0 + ")")).Click();
            }

            [When(@"I click the ""(.*)"" button")]
            public void WhenIClickTheButton(string p0)
            {
                System.Threading.Thread.Sleep(1000);
                switch (p0)
                {
                    case "Compare":
                        _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("selectedCompareTable")));
                        _driver.FindElement(By.Id("selectedCompareTable")).FindElement(By.CssSelector("button:last-child")).Click();
                        break;
                    case "Popular Comparsion One":
                        _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("popCompareBtn1")));
                        _driver.FindElement(By.Id("popCompareBtn1")).Click();
                        break;
                }
            }


            [When(@"I click on a Make '(.*)'")]
            public void WhenIClickOnAMake(string p0)
            {
                var id = p0.ToLower() + "_st";
                _driver.Navigate().GoToUrl(_driver.FindElement(By.Id(id)).GetAttribute("href"));
            }

            [When(@"I click on Category '(.*)'")]
            public void WhenIClickOnCategory(string p0)
            {
                var id = p0.ToLower() + "_st";
                _driver.Navigate().GoToUrl(_driver.FindElement(By.Id(id)).GetAttribute("href"));
            }

            [When(@"I click on a Model '(.*)' '(.*)'")]
            public void WhenIClickOnAModel(string p0, string p1)
            {
                if (ScenarioContext.Current.ScenarioInfo.Tags.Contains("Research"))
                {
                    var cssSelector = "[title='" + p0 + " " + p1 + "']";
                    _driver.Navigate().GoToUrl(_driver.FindElement(By.CssSelector(cssSelector)).GetAttribute("href"));
                }
                else if (ScenarioContext.Current.ScenarioInfo.Tags.Contains("CarsForSale"))
                {
                    var id = p1.ToLower() + "_st";
                    _driver.Navigate().GoToUrl(_driver.FindElement(By.Id(id)).GetAttribute("href"));
                }
            }

            [When(@"I click on a Super Trim '(.*)'")]
            public void WhenIClickOnASuperTrim(string p0)
            {
                _driver.FindElement(By.Id(p0)).Click();
            }

            [When(@"I click on a Real Trim '(.*)'")]
            public void WhenIClickOnARealTrim(string p0)
            {
                _driver.Navigate().GoToUrl(_driver.FindElement(By.Id(p0)).FindElement(By.TagName("a")).GetAttribute("href"));
            }

            [When(@"I enter the Zip Code")]
            public void WhenIEnterTheZipCode()
            {
                _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ncLeadZipcode")));
                _driver.FindElement(By.Id("ncLeadZipcode")).SendKeys("99999");
            }

            [When(@"I Provide Info")]
            public void WhenIProvideInfo(Table table)
            {
                foreach (var row in table.Rows)
                {
                    _driver.FindElement(By.Id(row["id"])).Clear();
                    _driver.FindElement(By.Id(row["id"])).SendKeys(row["value"]);
                    System.Threading.Thread.Sleep(2000);
                }
            }

            [When(@"I select a Make/Mode/Trim")]
            public void WhenISelectAMakeModeTrim(Table table)
            {
                foreach (var row in table.Rows)
                {
                    _driver.FindElement(By.Id(row["id"])).SendKeys(row["value"]);
                    System.Threading.Thread.Sleep(2000);
                }
            }


            [When(@"I click on a Vehicle")]
            public void WhenIClickOnAVehicle()
            {
                _wait.Until((ExpectedConditions.ElementExists(By.Id("c4s_list_js"))));
                _driver.FindElement(By.Id("c4s_list_js")).FindElement(By.CssSelector("li:first-child a")).Click();
            }

            [When(@"I click on the first Car Buying Guide")]
            public void WhenIClickOnTheFirstCarBuyingGuide()
            {
                _driver.FindElement(By.Id("articles_wrapper_st")).FindElement(By.CssSelector("li:nth-child(1)")).Click();
            }

            [When(@"I click through the article")]
            public void WhenIClickThroughTheArticle()
            {
                while(_driver.FindElement(By.Id("next")).Displayed)
                {
                    _driver.FindElement(By.Id("next")).Click();
                }
            }

            #endregion When

            #region Then

            [Then(@"the ""(.*)"" page should be displayed")]
            public void ThenThePageShouldBeDisplayed(string p0)
            {
                var expectedUrl = "";

                switch (p0)
                {
                    case "Cars for Sale":
                        expectedUrl = _siteDomain + "cars-for-sale/";
                        break;
                    case "Home":
                        expectedUrl = _siteDomain;
                        break;
                    case "Research":
                        expectedUrl = _siteDomain + "car-research/";
                        break;
                    case "Results":
                        expectedUrl = _siteDomain + "cars-for-sale/results/";
                        break;
                    case "Tools":
                        expectedUrl = _siteDomain + "tools/";
                        break;
                    case "Car Comparison":
                        expectedUrl = _siteDomain + "tools/car-comparison/";
                        break;
                    case "Car Comparison Results":
                        expectedUrl = _siteDomain + "tools/car-comparison/results/";
                        break;
                    case "Car Payment Calculator":
                        expectedUrl = _siteDomain + "tools/calculators/payment-calculator/";
                        break;
                }

                Assert.AreEqual(expectedUrl, _driver.Url);
            }

            [Then(@"the page should scroll down to the ""(.*)"" section")]
            public void ThenThePageShouldScrollDownToTheSection(string p0)
            {
                Assert.AreNotEqual("0", _driver.Manage().Window.Position.Y);
            }

            [Then(@"the Url should change to '(.*)'")]
            public void ThenTheUrlShouldChangeTo(string p0)
            {
                var expectedUrl = "";

                if (ScenarioContext.Current.ScenarioInfo.Tags.Contains("ByMake"))
                {
                    expectedUrl = _siteDomain + p0 + "/";
                }
                else if (ScenarioContext.Current.ScenarioInfo.Tags.Contains("ByCategory"))
                {
                    expectedUrl = _siteDomain + p0 + "s/";
                }
                else if (ScenarioContext.Current.ScenarioInfo.Tags.Contains("CarsForSale"))
                {
                    expectedUrl = _siteDomain + "cars-for-sale/" + p0 + "/";
                }

                Assert.AreEqual(expectedUrl.ToLower(), _driver.Url);
            }

            [Then(@"the Url should change to '(.*)'/'(.*)'/")]
            public void ThenTheUrlShouldChangeTo(string p0, string p1)
            {
                var expectedUrl = _siteDomain + p0 + "/" + p1 + "/";
                Assert.AreEqual(expectedUrl.ToLower(), _driver.Url);
            }

            [Then(@"the '(.*)' should be visible")]
            public void ThenTheShouldBeVisible(string p0)
            {
                Assert.IsTrue(_driver.FindElement(By.Id(p0)).Displayed);    
            }

            [Then(@"the Change Car Overlay should be visible")]
            public void ThenTheChangeCarOverlayShouldBeVisible()
            {
                Assert.IsTrue(_driver.FindElement(By.Id("add_change_car_overlay_js")).Displayed);
            }


            [Then(@"the '(.*)' should be visible if no zip code is cached")]
            public void ThenTheShouldBeVisibleIfNoZipCodeIsCached(string p0)
            {
                if (_driver.FindElement(By.Id(p0)).GetAttribute("value").IsNullOrEmpty())
                {
                    Assert.IsTrue(_driver.FindElement(By.Id(p0)).Displayed);
                }
            }


            [Then(@"the Url should change to the following '(.*)'/'(.*)'/'(.*)'/'(.*)'/")]
            public void ThenTheUrlShouldChangeToTheFollowing(string p0, string p1, int p2, string p3)
            {
                var expectedUrl = _siteDomain + p0 + "/" + p1 + "/" + p2 + "/" + p3 + "/";
                Assert.AreEqual(expectedUrl.ToLower(), _driver.Url);
            }

            [Then(@"the side panel should expand")]
            public void ThenTheSidePanelShouldExpand()
            {
                Assert.IsTrue(_driver.FindElement(By.Id("js_side_panel")).GetAttribute("class").Equals("side-panel-container"));
            }

            [Then(@"the leadform overlay should be displayed")]
            public void ThenTheLeadformOverlayShouldBeDisplayed()
            {
                Assert.IsTrue(_driver.FindElement(By.Id("leadform_overlay_js")).Displayed);
            }

            [Then(@"the Dealer should be Autobytel Test Toyota")]
            public void ThenTheDealerShouldBeAutobytelTestToyota()
            {
                _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("DealerList")));
                // Need to find a selector that works.
                //Assert.IsTrue(_driver.FindElement(By.CssSelector("#DealerList li:nth-of-type(1) input")).GetAttribute("checked").Equals("true"));
            }

            [Then(@"the PR Number should be displayed")]
            public void ThenThePRNumberShouldBeDisplayed()
            {
                _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("prnumber_st")));
                Assert.IsTrue(_driver.FindElement(By.Id("prnumber_st")).Displayed);
            }

            [Then(@"the Payment Estimate should be visible")]
            public void ThenThePaymentEstimateShouldBeVisible()
            {
                Assert.IsTrue(_driver.FindElement(By.Id("PaymentEstimate_monthlyPayment")).Displayed);
            }

            [Then(@"I should see the Next Articles page")]
            public void ThenIShouldSeeTheNextArticlesPage()
            {
                Assert.IsFalse(_driver.FindElement(By.Id("next")).Displayed);
            }
            
            #endregion Then

        #endregion Tests
    }
}
