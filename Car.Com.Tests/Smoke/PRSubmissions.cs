using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Web.UI;
using Car.Com.Domain.Models.VehicleSpec;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.UI;

namespace Car.Com.Tests.Smoke
{
  public class PrSubmissions
  {
    private IWebDriver _driver;
    private readonly string _browser;
    private readonly string _siteDomain;
    private string _results { get; set; }
    private string _confirmationNumber { get; set; }
    private string _driverLocation;
      private string _userAgent;


    public PrSubmissions()
    {
        _browser = "Chrome";
        _siteDomain = "http://localhost:8080/";
    }


    [SetUp]
    public virtual void Setup()
    {
      // Path for the drivers changes depending on the target system.
      //_driverLocation = Environment.MachineName == "SSCUTIL201"
      //  ? @"D:\Scripts\bin"
      //  : @"C:\Scripts\bin";

      // Creates the Web Driver object based on the browser
      switch (_browser)
      {
          case "IE":
              // Currently cannot set user-agent for IE
              _driver = _driverLocation == null ? new InternetExplorerDriver() : new InternetExplorerDriver(_driverLocation);
              break;
          
          case "Chrome":
              if (String.IsNullOrEmpty(_userAgent))
              {
                  var options = new ChromeOptions();
                  options.AddArgument("--user-agent=" + _userAgent);    
              }
              _driver = _driverLocation == null ? new ChromeDriver() : new ChromeDriver(_driverLocation);
              break;
          
          case "PhantomJS":
              if (String.IsNullOrEmpty(_userAgent))
              {
                  var options = new PhantomJSOptions();
                  options.AddAdditionalCapability("phantomjs.page.settings.userAgent", _userAgent);
              }
              _driver = _driverLocation == null ? new PhantomJSDriver() : new PhantomJSDriver(_driverLocation);
              break;
          
          default:
              _driver = _driverLocation == null ? new InternetExplorerDriver() : new InternetExplorerDriver(_driverLocation);
              break;
      }
    }
    

    [TearDown]
    public virtual void TearDown()
    {
      _driver.Close();
      _driver.Dispose();
    }



    [Test]
    public void FirstPracticeTest()
    {
      const string url = "cars-for-sale/";

      _driver.Navigate().GoToUrl(_siteDomain);
      System.Threading.Thread.Sleep(3000);

      _driver.Navigate().GoToUrl(_siteDomain + url);
      System.Threading.Thread.Sleep(3000);
    }

    // Car.com Smoke Test Section 1: New PR Submission by Make
    [Test]
    public void NewPrSubmissionByMake()
    {
        const string baseurl = "http://w3.dev.car.com";

        _driver.Navigate().GoToUrl(baseurl);
        Assert.AreEqual(baseurl + "/", _driver.Url);
        
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
        
        System.Threading.Thread.Sleep(2000);

        // Click Research
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("a.hero-btn.one")));
        Assert.IsTrue(_driver.FindElement(By.CssSelector("a.hero-btn.one")).GetAttribute("title").Equals("Research"));
        _driver.FindElement(By.CssSelector("a.hero-btn.one")).Click();
        Assert.AreEqual(baseurl + "/car-research/", _driver.Url);

        System.Threading.Thread.Sleep(2000);

        // Click By Make
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("a.hero-btn.one")));
        //Assert.IsTrue(_driver.FindElement(By.CssSelector("a.hero-btn.one")).GetAttribute("title").Equals("By Make"));
        _driver.FindElement(By.CssSelector("a.hero-btn.one")).Click();
        Assert.AreNotEqual("0", _driver.Manage().Window.Position.Y);

        System.Threading.Thread.Sleep(2000);

        // Select Make - Honda
        // This method of navigating the site is used as this element does not take a .Click() event.
        var makemodeltrimurl = _driver.FindElement(By.CssSelector("a[href='/honda/']")).GetAttribute("href");
        _driver.Navigate().GoToUrl(makemodeltrimurl);
        Assert.AreEqual(makemodeltrimurl, _driver.Url);

        System.Threading.Thread.Sleep(2000);

        // Select Model - Accord
        // This method of navigating the site is used as this element does not take a .Click() event.
        makemodeltrimurl = _driver.FindElement(By.CssSelector("a[href='/honda/accord/']")).GetAttribute("href");
        _driver.Navigate().GoToUrl(makemodeltrimurl);
        Assert.AreEqual(makemodeltrimurl, _driver.Url);

        System.Threading.Thread.Sleep(2000);

        // Select Super Trim - Crosstourxv6
        _driver.FindElement(By.Id("js-disablesupertrim-crosstourexv6")).Click();
        Assert.IsTrue(_driver.FindElement(By.Id("overlayjs-trimcollection-crosstourexv6")).Displayed);

        System.Threading.Thread.Sleep(2000);

        // Select Real Trim - 2015
        // This method of navigating the site is used as this element does not take a .Click() event.
        makemodeltrimurl = _driver.FindElement(By.CssSelector("a[href='/honda/accord/2015/crosstour-ex-v6-(a6)-fwd/']")).GetAttribute("href");
        _driver.Navigate().GoToUrl(makemodeltrimurl);
        Assert.AreEqual(makemodeltrimurl, _driver.Url);

        System.Threading.Thread.Sleep(2000);

        // Click on Pricing & Offers
        _driver.FindElement(By.CssSelector("a.block.js_pricing_offers_init_btn")).Click();
        Assert.IsTrue(_driver.FindElement(By.Id("js_side_panel")).GetAttribute("class").Equals("side-panel-container"));

        System.Threading.Thread.Sleep(2000);

        // Enter Zipcode
        // The wait is used here, to wait for the overlay to be ready. Otherwise it fails.
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ncLeadZipcode")));
        _driver.FindElement(By.Id("ncLeadZipcode")).SendKeys("99999");
        Assert.IsTrue(_driver.FindElement(By.Id("ncLeadZipcode")).GetAttribute("value").Equals("99999"));

        System.Threading.Thread.Sleep(2000);

        // Click on Continue to Get Pricing
        _driver.FindElement(By.Id("GetDealersButton")).Click();
        Assert.IsTrue(_driver.FindElement(By.Id("leadform_overlay_js")).Displayed);

        System.Threading.Thread.Sleep(2000);

        // Select a Dealer
        wait.Until((ExpectedConditions.ElementIsVisible(By.Id("147374^^1"))));
        Assert.IsTrue(_driver.FindElement(By.Id("147374^^1")).GetAttribute("checked").Equals("true"));

        System.Threading.Thread.Sleep(2000);

        // Enter First Name
        _driver.FindElement(By.Id("fName")).SendKeys("Bob");

        System.Threading.Thread.Sleep(2000);

        // Enter Last Name
        _driver.FindElement(By.Id("lName")).SendKeys("Generic");

        System.Threading.Thread.Sleep(2000);

        // Enter Street Address
        _driver.FindElement(By.Id("strAddr")).SendKeys("18872 MacArthur Blvd Suite 200");

        System.Threading.Thread.Sleep(2000);

        // Enter Home Phone Area Code
        _driver.FindElement(By.Id("HParea")).SendKeys("949");

        System.Threading.Thread.Sleep(2000);
        
        // Enter Home Phone Pre
        _driver.FindElement(By.Id("HPpre")).SendKeys("225");

        System.Threading.Thread.Sleep(2000);
        
        // Enter Home Phone Phone
        _driver.FindElement(By.Id("HPphone")).SendKeys("4500");

        System.Threading.Thread.Sleep(2000);
        
        // Enter Email Address
        _driver.FindElement(By.Id("email")).SendKeys("testlead@autobytel.com");

        System.Threading.Thread.Sleep(2000);
        
        // Click on Submit and Get Pricing
        _driver.FindElement(By.Id("SubmitPostButton")).Click();

        System.Threading.Thread.Sleep(2000);

        // Get Confirmation Number
        Assert.IsTrue(_driver.FindElement(By.Id("prnumber_st")).Displayed);
        _confirmationNumber = _driver.FindElement(By.Id("prnumber_st")).Text;
        Console.WriteLine(_confirmationNumber);

        System.Threading.Thread.Sleep(5000);

    }
  }
}
