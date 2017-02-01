using System;
using Car.Com.Common;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.UI;

namespace Car.Com.Tests
{
    [TestFixture]
    public class SmokeTest
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;

        public string Browser { get; set; }
        public string SiteDomain { get; set; }
        private string UserAgent { get; set; }
        public string Result { get; set; }
        public string Transcript { get; set; }
        public string ConfirmationNumber { get; set; }
        private string _driverLocation;

        #region Constructor

        public SmokeTest()
        {
            Browser = "Chrome";
            SiteDomain = "http://w3.dev.car.com/";
        }

        public SmokeTest(string browser, string sitedomain)
        {
            Browser = browser;
            SiteDomain = sitedomain;
        }

        #endregion Constructor

        #region SetUp & TearDown

        [SetUp]
        public virtual void SetUp()
        {
            switch (Browser)
            {
                case "Chrome":
                    var chromeOptions = new ChromeOptions();

                    if (String.IsNullOrEmpty(UserAgent))
                    {
                        chromeOptions.AddArgument("--user-agent=" + UserAgent);
                    }

                    chromeOptions.AddArguments("start-maximized");
                    
                    //options.AddArguments("incognito");

                    _driver = _driverLocation == null ? new ChromeDriver(chromeOptions) : new ChromeDriver(_driverLocation, chromeOptions);
                    
                    break;

                case "IE":
                    _driver = _driverLocation == null ? new InternetExplorerDriver() : new InternetExplorerDriver(_driverLocation);
                    
                    break;

                case "PhantomJS":
                    var phandomJsOptions = new PhantomJSOptions();

                    if (String.IsNullOrEmpty(UserAgent))
                    {
                        phandomJsOptions.AddAdditionalCapability("phantomjs.page.settings.userAgent", UserAgent);
                    }

                    _driver = _driverLocation == null ? new PhantomJSDriver(phandomJsOptions) : new PhantomJSDriver(_driverLocation, phandomJsOptions);
                    _driver.Manage().Window.Maximize();

                    break;
            }

            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

            Result = "Fail";
        }

        [TearDown]
        public virtual void TearDown()
        {
            _driver.Close();
            _driver.Dispose();
        }

        #endregion SetUp & TearDown

        #region SectionOne

        // PR Submission Research By Make
        [Test]
        public void SectionOne()
        {
            _driver.Navigate().GoToUrl(SiteDomain);
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain, _driver.Url);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("research_st")));
            _driver.FindElement(By.Id("research_st")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain + "car-research/", _driver.Url);
            Transcript += "1. Click on Research. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("by_make_st")));
            _driver.FindElement(By.Id("by_make_st")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual("0", _driver.Manage().Window.Position.Y);
            Transcript += "2. Click on By Make. \n";

            _driver.Navigate().GoToUrl(_driver.FindElement(By.Id("honda_st")).GetAttribute("href"));
            Assert.AreEqual(SiteDomain + "honda/", _driver.Url);
            Transcript += "3. Select make Honda. \n";

            _driver.Navigate().GoToUrl(_driver.FindElement(By.Id("honda_accord_st")).GetAttribute("href"));
            Assert.AreEqual(SiteDomain + "honda/accord/", _driver.Url);
            Transcript += "4. Select model Accord. \n";

            _driver.FindElement(By.Id("js-disablesupertrim-accordex")).Click();
            Assert.IsTrue(_driver.FindElement(By.Id("overlayjs-trimcollection-accordex")).Displayed);
            Transcript += "5. Select supertrim EX. \n";

            _driver.Navigate().GoToUrl(_driver.FindElement(By.Id("js-trimrow-35717")).FindElement(By.TagName("a")).GetAttribute("href"));
            Assert.AreEqual(SiteDomain + "honda/accord/2015/ex-(m6)-sedan/", _driver.Url);
            Transcript += "6. Select real trim 2015 Honda Accord EX (M6) Sedan. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("pno_btn_js")));
            _driver.FindElement(By.Id("pno_btn_js")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(_driver.FindElement(By.Id("offerpanel_wrap_js")).GetAttribute("class").Equals("side-panel"));
            Transcript += "7. Click Pricing & Offers. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ncLeadZipcode")));
            _driver.FindElement(By.Id("ncLeadZipcode")).Clear();
            _driver.FindElement(By.Id("ncLeadZipcode")).SendKeys("99999");
            Transcript += "8. Enter the Zip Code 99999. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("get_pricing_btn_js")));
            _driver.FindElement(By.Id("get_pricing_btn_js")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(_driver.FindElement(By.Id("leadform_overlay_js")).Displayed);   
            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("DealerList")));
            Transcript += "9. Click Continue to Get Pricing. \n";

            _driver.FindElement(By.Id("fName")).Clear();
            _driver.FindElement(By.Id("fName")).SendKeys("Bob");
            System.Threading.Thread.Sleep(2000);
            Transcript += "10. Enter the First Name Bob. \n";

            _driver.FindElement(By.Id("lName")).Clear();
            _driver.FindElement(By.Id("lName")).SendKeys("Generic");
            System.Threading.Thread.Sleep(2000);
            Transcript += "11. Enter the Last Name Generic. \n";

            _driver.FindElement(By.Id("strAddr")).Clear();
            _driver.FindElement(By.Id("strAddr")).SendKeys("18872 MacArthur Blvd");
            System.Threading.Thread.Sleep(2000);
            Transcript += "12. Enter the Address 18872 MacArthur Blvd. \n";

            _driver.FindElement(By.Id("HParea")).Clear();
            _driver.FindElement(By.Id("HParea")).SendKeys("949");
            System.Threading.Thread.Sleep(2000);

            _driver.FindElement(By.Id("HPpre")).Clear();
            _driver.FindElement(By.Id("HPpre")).SendKeys("225");
            System.Threading.Thread.Sleep(2000);

            _driver.FindElement(By.Id("HPphone")).Clear();
            _driver.FindElement(By.Id("HPphone")).SendKeys("4500");
            System.Threading.Thread.Sleep(2000);
            Transcript += "13. Enter the Phone Number 949-225-4500. \n";

            _driver.FindElement(By.Id("email")).Clear();
            _driver.FindElement(By.Id("email")).SendKeys("testlead@autobytel.com");
            System.Threading.Thread.Sleep(2000);
            Transcript += "14. Enter the Email testlead@autobytel.com. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("submit_lead_btn_js")));
            _driver.FindElement(By.Id("submit_lead_btn_js")).Click();
            System.Threading.Thread.Sleep(1000);
            Transcript += "15. Click Submit and Get Pricing. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("prnumber_st")));
            Assert.IsTrue(_driver.FindElement(By.Id("prnumber_st")).Displayed);
            
            ConfirmationNumber = _driver.FindElement(By.Id("prnumber_st")).Text;
            Transcript += "16. The Confirmation Number is " + ConfirmationNumber + ". \n";

            if (ConfirmationNumber.IsNotNullOrEmpty())
            {
                Result = "Pass";
            }
            else
            {
                Result = "Fail";
            }
        }

        #endregion SectionOne

        #region SectionTwo

        // PR Submission Research By Category
        [Test]
        public void SectionTwo()
        {
            _driver.Navigate().GoToUrl(SiteDomain);
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain, _driver.Url);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("research_st")));
            _driver.FindElement(By.Id("research_st")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain + "car-research/", _driver.Url);
            Transcript += "1. Click on Research. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("by_cat_st")));
            _driver.FindElement(By.Id("by_cat_st")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual("0", _driver.Manage().Window.Position.Y);
            Transcript += "2. Click on By Category. \n";

            _driver.Navigate().GoToUrl(_driver.FindElement(By.Id("sedan_st")).GetAttribute("href"));
            Assert.AreEqual(SiteDomain + "sedans/", _driver.Url);
            Transcript += "3. Select category Sedan. \n";

            _driver.FindElement(By.Id("js-disablesupertrim-accordex")).Click();
            Assert.IsTrue(_driver.FindElement(By.Id("overlayjs-trimcollection-accordex")).Displayed);
            Transcript += "4. Select supertrim EX. \n";

            _driver.Navigate().GoToUrl(_driver.FindElement(By.Id("js-trimrow-35717")).FindElement(By.TagName("a")).GetAttribute("href"));
            Assert.AreEqual(SiteDomain + "honda/accord/2015/ex-(m6)-sedan/", _driver.Url);
            Transcript += "5. Select real trim 2015 Honda Accord EX (M6) Sedan. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("pno_btn_js")));
            _driver.FindElement(By.Id("pno_btn_js")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(_driver.FindElement(By.Id("offerpanel_wrap_js")).GetAttribute("class").Equals("side-panel"));
            Transcript += "6. Click Pricing & Offers. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ncLeadZipcode")));
            _driver.FindElement(By.Id("ncLeadZipcode")).Clear();
            _driver.FindElement(By.Id("ncLeadZipcode")).SendKeys("99999");
            Transcript += "7. Enter the Zip Code 99999. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("get_pricing_btn_js")));
            _driver.FindElement(By.Id("get_pricing_btn_js")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(_driver.FindElement(By.Id("leadform_overlay_js")).Displayed);    
            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("DealerList")));
            Transcript += "8. Click Continue to Get Pricing. \n";

            _driver.FindElement(By.Id("fName")).Clear();
            _driver.FindElement(By.Id("fName")).SendKeys("Bob");
            System.Threading.Thread.Sleep(2000);
            Transcript += "9. Enter the First Name Bob. \n";

            _driver.FindElement(By.Id("lName")).Clear();
            _driver.FindElement(By.Id("lName")).SendKeys("Generic");
            System.Threading.Thread.Sleep(2000);
            Transcript += "10. Enter the Last Name Generic. \n";

            _driver.FindElement(By.Id("strAddr")).Clear();
            _driver.FindElement(By.Id("strAddr")).SendKeys("18872 MacArthur Blvd");
            System.Threading.Thread.Sleep(2000);
            Transcript += "11. Enter the Address 18872 MacArthur Blvd. \n";

            _driver.FindElement(By.Id("HParea")).Clear();
            _driver.FindElement(By.Id("HParea")).SendKeys("949");
            System.Threading.Thread.Sleep(2000);

            _driver.FindElement(By.Id("HPpre")).Clear();
            _driver.FindElement(By.Id("HPpre")).SendKeys("225");
            System.Threading.Thread.Sleep(2000);

            _driver.FindElement(By.Id("HPphone")).Clear();
            _driver.FindElement(By.Id("HPphone")).SendKeys("4500");
            System.Threading.Thread.Sleep(2000);
            Transcript += "12. Enter the Phone Number 949-225-4500. \n";

            _driver.FindElement(By.Id("email")).Clear();
            _driver.FindElement(By.Id("email")).SendKeys("testlead@autobytel.com");
            System.Threading.Thread.Sleep(2000);
            Transcript += "13. Enter the Email testlead@autobytel.com. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("submit_lead_btn_js")));
            _driver.FindElement(By.Id("submit_lead_btn_js")).Click();
            System.Threading.Thread.Sleep(1000);
            Transcript += "14. Click Submit and Get Pricing. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("prnumber_st")));
            Assert.IsTrue(_driver.FindElement(By.Id("prnumber_st")).Displayed);

            ConfirmationNumber = _driver.FindElement(By.Id("prnumber_st")).Text;
            Transcript += "15. The Confirmation Number is " + ConfirmationNumber + ". \n";

            if (ConfirmationNumber.IsNotNullOrEmpty())
            {
                Result = "Pass";
            }
            else
            {
                Result = "Fail";
            }
        }

        #endregion SectionTwo

        #region SectionThree

        // PR Submission Cars For Sale By Make
        [Test]
        public void SectionThree()
        {
            _driver.Navigate().GoToUrl(SiteDomain);
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain, _driver.Url);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("c4s_st")));
            _driver.FindElement(By.Id("c4s_st")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain + "cars-for-sale/", _driver.Url);
            Transcript += "1. Click Cars For Sale. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("by_make_st")));
            _driver.FindElement(By.Id("by_make_st")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual("0", _driver.Manage().Window.Position.Y);
            Transcript += "2. Click By Make. \n";

            // Select the Honda Make
            _driver.FindElement(By.Id("honda_st")).Click();
            _driver.FindElement(By.Id("btnChooseModels")).Click();
            Assert.AreEqual(SiteDomain + "cars-for-sale/selectmodels/", _driver.Url);
            Transcript += "3. Select make Honda. \n";

            // Select the Accord Model
            _driver.FindElement(By.Id("Accord_st")).Click();
            _driver.FindElement(By.Id("btnShowResults")).Click();
            Assert.AreEqual(SiteDomain + "cars-for-sale/results/", _driver.Url);
            Transcript += "4. Select model Accord. \n";

            if (_driver.FindElement(By.Id("zip_overlay_js")).Displayed)
            {
                _driver.FindElement(By.Id("zip_js")).Clear();
                _driver.FindElement(By.Id("zip_js")).SendKeys("92612");
                System.Threading.Thread.Sleep(2000);

                _driver.FindElement(By.Id("radius_js")).Clear();
                _driver.FindElement(By.Id("radius_js")).SendKeys("25");
                System.Threading.Thread.Sleep(2000);
                
                // query_btn_st ID does not exist yet
                _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("query_btn_st")));
                _driver.FindElement(By.Id("query_btn_st")).Click();
                System.Threading.Thread.Sleep(1000);
                // Assert Needed
            }
            Transcript += "5. Check if Zip & Distance overlay is displayed. \n";
            
            _wait.Until((ExpectedConditions.ElementExists(By.Id("c4s_list_js"))));
            _driver.FindElement(By.Id("c4s_list_js")).FindElement(By.CssSelector("li:first-child a")).Click();
            // Assert Needed
            Transcript += "6. Select first Vehicle entry. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("pno_btn_js")));
            _driver.FindElement(By.Id("pno_btn_js")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(_driver.FindElement(By.Id("offerpanel_wrap_js")).GetAttribute("class").Equals("side-panel"));
            Transcript += "7. Click Pricing & Offers. \n";

            // get_pricing_btn_js ID does not exist yet, which would make this easier to do
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#sp_c4s_Lead p:first a")));
            _driver.FindElement(By.Id("sp_c4s_Lead")).FindElement(By.CssSelector("p:first a")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(_driver.FindElement(By.Id("leadform_overlay_js")).Displayed);
            Transcript += "8. Click Contact the Seller. \n";

            _driver.FindElement(By.Id("fName")).Clear();
            _driver.FindElement(By.Id("fName")).SendKeys("Bob");
            System.Threading.Thread.Sleep(2000);
            Transcript += "9. Enter the First Name Bob. \n";

            _driver.FindElement(By.Id("lName")).Clear();
            _driver.FindElement(By.Id("lName")).SendKeys("Generic");
            System.Threading.Thread.Sleep(2000);
            Transcript += "10. Enter the Last Name Generic. \n";

            _driver.FindElement(By.Id("strAddr")).Clear();
            _driver.FindElement(By.Id("strAddr")).SendKeys("18872 MacArthur Blvd");
            System.Threading.Thread.Sleep(2000);
            Transcript += "11. Enter the Address 18872 MacArthur Blvd. \n";

            _driver.FindElement(By.Id("HParea")).Clear();
            _driver.FindElement(By.Id("HParea")).SendKeys("949");
            System.Threading.Thread.Sleep(2000);

            _driver.FindElement(By.Id("HPpre")).Clear();
            _driver.FindElement(By.Id("HPpre")).SendKeys("225");
            System.Threading.Thread.Sleep(2000);

            _driver.FindElement(By.Id("HPphone")).Clear();
            _driver.FindElement(By.Id("HPphone")).SendKeys("4500");
            System.Threading.Thread.Sleep(2000);
            Transcript += "12. Enter the Phone Number 949-225-4500. \n";

            _driver.FindElement(By.Id("email")).Clear();
            _driver.FindElement(By.Id("email")).SendKeys("testlead@autobytel.com");
            System.Threading.Thread.Sleep(2000);
            Transcript += "13. Enter the Email testlead@autobytel.com. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("submit_lead_btn_js")));
            _driver.FindElement(By.Id("submit_lead_btn_js")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(_driver.FindElement(By.Id("ol_ofr_TY_LeadInfo")).Displayed);
            Transcript += "14. Click Contact this Dealer. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("prnumber_st")));
            Assert.IsTrue(_driver.FindElement(By.Id("prnumber_st")).Displayed);

            ConfirmationNumber = _driver.FindElement(By.Id("prnumber_st")).Text;
            Transcript += "15. The Confirmation Number is " + ConfirmationNumber + ". \n";

            if (ConfirmationNumber.IsNotNullOrEmpty())
            {
                Result = "Pass";
            }
            else
            {
                Result = "Fail";
            }
        }

        #endregion SectionThree

        #region SectionFour

        // PR Submission Cars For Sale By Category
        [Test]
        public void SectionFour()
        {
            _driver.Navigate().GoToUrl(SiteDomain);
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain, _driver.Url);

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("c4s_st")));
            _driver.FindElement(By.Id("c4s_st")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain + "cars-for-sale/", _driver.Url);
            Transcript += "1. Click Cars For Sale. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("by_cat_st")));
            _driver.FindElement(By.Id("by_cat_st")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual("0", _driver.Manage().Window.Position.Y);
            Transcript += "2. Click By Category. \n";

            _driver.FindElement(By.Id("sedan")).Click();
            _driver.FindElement(By.Id("btnChooseMakes")).Click();
            Assert.AreEqual(SiteDomain + "cars-for-sale/selectcategorymakes/", _driver.Url);
            Transcript += "3. Select Sedans. \n";

            _driver.FindElement(By.Id("4_Honda_st")).Click();
            _driver.FindElement(By.Id("btnShowResults")).Click();
            Transcript += "4. Select Makes. \n";

            if (_driver.FindElement(By.Id("zip_overlay_js")).Displayed)
            {
                _driver.FindElement(By.Id("zip_js")).Clear();
                _driver.FindElement(By.Id("zip_js")).SendKeys("92612");
                System.Threading.Thread.Sleep(2000);

                _driver.FindElement(By.Id("radius_js")).Clear();
                _driver.FindElement(By.Id("radius_js")).SendKeys("25");
                System.Threading.Thread.Sleep(2000);

                // query_btn_st ID does not exist yet
                _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("query_btn_st")));
                _driver.FindElement(By.Id("query_btn_st")).Click();
                System.Threading.Thread.Sleep(1000);
                // Assert Needed
            }
            Transcript += "5. Check if Zip & Distance overlay is displayed. \n";

            _wait.Until((ExpectedConditions.ElementExists(By.Id("c4s_list_js"))));
            _driver.FindElement(By.Id("c4s_list_js")).FindElement(By.CssSelector("li:first-child a")).Click();
            // Assert Needed
            Transcript += "6. Select first Vehicle entry. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("pno_btn_js")));
            _driver.FindElement(By.Id("pno_btn_js")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(_driver.FindElement(By.Id("offerpanel_wrap_js")).GetAttribute("class").Equals("side-panel"));
            Transcript += "7. Click Pricing & Offers. \n";

            // get_pricing_btn_js ID does not exist yet
            _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#sp_c4s_Lead p:first a")));
            _driver.FindElement(By.Id("sp_c4s_Lead")).FindElement(By.CssSelector("p:first a")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(_driver.FindElement(By.Id("leadform_overlay_js")).Displayed);
            Transcript += "8. Click Contact the Seller. \n";

            _driver.FindElement(By.Id("fName")).Clear();
            _driver.FindElement(By.Id("fName")).SendKeys("Bob");
            System.Threading.Thread.Sleep(2000);
            Transcript += "9. Enter the First Name Bob. \n";

            _driver.FindElement(By.Id("lName")).Clear();
            _driver.FindElement(By.Id("lName")).SendKeys("Generic");
            System.Threading.Thread.Sleep(2000);
            Transcript += "10. Enter the Last Name Generic. \n";

            _driver.FindElement(By.Id("strAddr")).Clear();
            _driver.FindElement(By.Id("strAddr")).SendKeys("18872 MacArthur Blvd");
            System.Threading.Thread.Sleep(2000);
            Transcript += "11. Enter the Address 18872 MacArthur Blvd. \n";

            _driver.FindElement(By.Id("HParea")).Clear();
            _driver.FindElement(By.Id("HParea")).SendKeys("949");
            System.Threading.Thread.Sleep(2000);

            _driver.FindElement(By.Id("HPpre")).Clear();
            _driver.FindElement(By.Id("HPpre")).SendKeys("225");
            System.Threading.Thread.Sleep(2000);

            _driver.FindElement(By.Id("HPphone")).Clear();
            _driver.FindElement(By.Id("HPphone")).SendKeys("4500");
            System.Threading.Thread.Sleep(2000);
            Transcript += "12. Enter the Phone Number 949-225-4500. \n";

            _driver.FindElement(By.Id("email")).Clear();
            _driver.FindElement(By.Id("email")).SendKeys("testlead@autobytel.com");
            System.Threading.Thread.Sleep(2000);
            Transcript += "13. Enter the Email testlead@autobytel.com. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("submit_lead_btn_js")));
            _driver.FindElement(By.Id("submit_lead_btn_js")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(_driver.FindElement(By.Id("ol_ofr_TY_LeadInfo")).Displayed);
            Transcript += "14. Click Contact this Dealer. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("prnumber_st")));
            Assert.IsTrue(_driver.FindElement(By.Id("prnumber_st")).Displayed);

            ConfirmationNumber = _driver.FindElement(By.Id("prnumber_st")).Text;
            Transcript += "15. The Confirmation Number is " + ConfirmationNumber + ". \n";

            if (ConfirmationNumber.IsNotNullOrEmpty())
            {
                Result = "Pass";
            }
            else
            {
                Result = "Fail";
            }
        }

        #endregion SectionFour

        #region SectionFive

        // Car Comparisons Choose Cars
        [Test]
        public void SectionFive()
        {
            _driver.Navigate().GoToUrl(SiteDomain + "tools/");
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain + "tools/", _driver.Url);
            Transcript += "1. Start on the Tools page. \n";
            
            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("comparisons_btn_st")));
            _driver.FindElement(By.Id("comparisons_btn_st")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual("0", _driver.Manage().Window.Position.Y);
            Transcript += "2. Click Car Comparisons. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("custom_comparisons_btn_st")));
            _driver.FindElement(By.Id("custom_comparisons_btn_st")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain + "tools/car-comparison/", _driver.Url);
            Transcript += "3. Click Begin Your Comparison. \n";
            
            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("selectedCompareTable")));
            _driver.FindElement(By.Id("selectedCompareTable")).FindElement(By.CssSelector("button:nth-child(1)")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(_driver.FindElement(By.Id("add_change_car_overlay_js")).Displayed);
            Transcript += "4. Click first Select a Car. \n";

            _driver.FindElement(By.Id("compareCarMakeSelect")).SendKeys("honda");
            System.Threading.Thread.Sleep(2000);
            Transcript += "5. Choose make Honda. \n";

            _driver.FindElement(By.Id("compareCarModelSelect")).SendKeys("accord");
            System.Threading.Thread.Sleep(2000);
            Transcript += "6. Choose model Accord. \n";

            _driver.FindElement(By.Id("compareCarTrimSelect")).SendKeys("2015 EX (CVT) Coup");
            System.Threading.Thread.Sleep(2000);
            Transcript += "7. Choose vehicle 2015 EX (CVT) Coup. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("selectedCompareTable")));
            _driver.FindElement(By.Id("selectedCompareTable")).FindElement(By.CssSelector("button:nth-child(2)")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(_driver.FindElement(By.Id("add_change_car_overlay_js")).Displayed);
            Transcript += "8. Click second Select a Car. \n";

            _driver.FindElement(By.Id("compareCarMakeSelect")).SendKeys("toyota");
            System.Threading.Thread.Sleep(2000);
            Transcript += "9. Choose make Toyota. \n";

            _driver.FindElement(By.Id("compareCarModelSelect")).SendKeys("camry");
            System.Threading.Thread.Sleep(2000);
            Transcript += "10. Choose model Camry. \n";

            _driver.FindElement(By.Id("compareCarTrimSelect")).SendKeys("2015 LE");
            System.Threading.Thread.Sleep(2000);
            Transcript += "11. Choose vehicle 2015 LE. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("compare_btn_st")));
            _driver.FindElement(By.Id("compare_btn_st")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain + "tools/car-comparison/results/", _driver.Url);

            Transcript += "12. Click Compare. \n";
            Transcript += "13. Confirm Comparison is shown. \n";
            Result = "Pass";
        }

        #endregion SectionFive

        // Still Failing
        #region SectionSix

        // Car Comparisons Popular Comparisons
        [Test]
        public void SectionSix()
        {
            _driver.Navigate().GoToUrl(SiteDomain + "tools/");
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain + "tools/", _driver.Url);
            Transcript += "1. Start on the Tools page. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("comparisons_btn_st")));
            _driver.FindElement(By.Id("comparisons_btn_st")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual("0", _driver.Manage().Window.Position.Y);
            Transcript += "2. Click Car Comparisons. \n";

            //_wait.Until(ExpectedConditions.ElementIsVisible(By.Id("popCompareBtn1")));
            _driver.FindElement(By.Id("popCompareBtn1")).Click();
            System.Threading.Thread.Sleep(1000);
            Transcript += "3. Click first Popular Comparison. \n";

            Assert.AreEqual(SiteDomain + "tools/car-comparison/results/", _driver.Url);
            Transcript += "4. Confirm Popular Comparison is shown. \n";
            Result = "Pass";

        }

        #endregion SectionSix

        #region SectionSeven

        // Car Payment Calculators
        [Test]
        public void SectionSeven()
        {
            _driver.Navigate().GoToUrl(SiteDomain + "tools/");
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain + "tools/", _driver.Url);
            Transcript += "1. Start on the Tools page. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("calculators_btn_st")));
            _driver.FindElement(By.Id("calculators_btn_st")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.AreNotEqual("0", _driver.Manage().Window.Position.Y);
            Transcript += "2. Click Payment Calculators. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("payment_btn_st")));
            _driver.FindElement(By.Id("payment_btn_st")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain + "tools/calculators/payment-calculator/", _driver.Url);
            Transcript += "3. Click Car Payment Calculator. \n";

            _driver.FindElement(By.Id("PaymentEstimate_purchasePrice")).Clear();
            _driver.FindElement(By.Id("PaymentEstimate_purchasePrice")).SendKeys("32000");
            System.Threading.Thread.Sleep(2000);
            Transcript += "4. Enter the Purchase Price of $32,000. \n";

            _driver.FindElement(By.Id("PaymentEstimate_cashRebate")).Clear();
            _driver.FindElement(By.Id("PaymentEstimate_cashRebate")).SendKeys("0");
            System.Threading.Thread.Sleep(2000);
            Transcript += "5. Enter the Cash Rebate of $0. \n";

            _driver.FindElement(By.Id("PaymentEstimate_tradeIn")).Clear();
            _driver.FindElement(By.Id("PaymentEstimate_tradeIn")).SendKeys("0");
            System.Threading.Thread.Sleep(2000);
            Transcript += "6. Enter the Value of Your Trade-In of $0. \n";

            _driver.FindElement(By.Id("PaymentEstimate_tradeInOwed")).Clear();
            _driver.FindElement(By.Id("PaymentEstimate_tradeInOwed")).SendKeys("0");
            System.Threading.Thread.Sleep(2000);
            Transcript += "7. Enter the Amount Owed on Your Trade-In of $0. \n";

            _driver.FindElement(By.Id("PaymentEstimate_downPayment")).Clear();
            _driver.FindElement(By.Id("PaymentEstimate_downPayment")).SendKeys("3000");
            System.Threading.Thread.Sleep(2000);
            Transcript += "8. Enter the Down Payment of $3,000. \n";

            _driver.FindElement(By.Id("PaymentEstimate_interestRate")).Clear();
            _driver.FindElement(By.Id("PaymentEstimate_interestRate")).SendKeys("3");
            System.Threading.Thread.Sleep(2000);
            Transcript += "9. Enter the Annual Interest Rate of 3%. \n";

            _driver.FindElement(By.Id("PaymentEstimate_termMonths")).Clear();
            _driver.FindElement(By.Id("PaymentEstimate_termMonths")).SendKeys("48");
            System.Threading.Thread.Sleep(2000);
            Transcript += "10. Enter the Term of Loan of 48 months. \n";

            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnCalculateMonthlyPayment")));
            _driver.FindElement(By.Id("btnCalculateMonthlyPayment")).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(_driver.FindElement(By.Id("PaymentEstimate_monthlyPayment")).Displayed);
            Transcript += "11. Click Calculate Monthly Payment. \n";

            var Amount = _driver.FindElement(By.Id("PaymentEstimate_monthlyPayment")).Text;
            Transcript += "12. The Result is" + Amount + ". \n";
            
            if (Amount.IsNotNullOrEmpty()) {
                Result = "Pass";
            } else {
                Result = "Fail";
            }
        }

        #endregion SectionSeven

        // Still Failing
        #region SectionEight

        // Buying Guides
        [Test]
        public void SectionEight()
        {
            _driver.Navigate().GoToUrl(SiteDomain);
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(SiteDomain, _driver.Url);

            _driver.Navigate().GoToUrl(_driver.FindElement(By.Id("articles_wrapper_st")).FindElement(By.CssSelector("li:nth-child(1) a")).GetAttribute("href"));
            System.Threading.Thread.Sleep(1000);
            Transcript += "1. Click the first Buying Guide Article. \n";

            while (_driver.FindElement(By.Id("next")).Displayed)
            {
                
                _driver.FindElement(By.Id("next")).Click();
                System.Threading.Thread.Sleep(1000);
            }
            Transcript += "2. Click through the Article. \n";

            System.Threading.Thread.Sleep(1000);
            Assert.IsFalse(_driver.FindElement(By.Id("next")).Displayed);

            Transcript += "3. Confirm list of other Buying Guide Articles are displayed. \n";
            Result = "Pass";
        }

        #endregion SectionEight
    }
}
