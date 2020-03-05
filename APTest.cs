using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;

[TestFixture]
public class APTest {
    private IWebDriver driver;
    public IDictionary<string, object> vars { get; private set; }

    
    private IJavaScriptExecutor js;
    [SetUp]
    public void SetUp() {
        driver = new ChromeDriver();
        js = (IJavaScriptExecutor)driver;
        vars = new Dictionary<string, object>();
    }
    [TearDown]
    protected void TearDown() {
        driver.Quit();
    }
    [Test]
  
  public void AP(string SelfIP, string CamIP, string pass, string gateway, string mask) {
        
            driver.Navigate().GoToUrl("https://192.168.1.20/login.cgi?uri=/");
            System.Windows.MessageBox.Show("Уберите проверку сертификата в хром и нажмите ок", "Пауза",
                                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information,
                                    System.Windows.MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
            driver.Manage().Window.Size = new System.Drawing.Size(1936, 1056);
            driver.FindElement(By.Id("username")).SendKeys("ubnt");
            driver.FindElement(By.Id("password")).SendKeys("ubnt");
            {
                var dropdown = driver.FindElement(By.Id("country"));
                dropdown.FindElement(By.XPath("//option[. = 'Russia']")).Click();
            }
            driver.FindElement(By.Id("agreed")).Click();
            driver.FindElement(By.CssSelector(".submit > input")).Click();
            driver.FindElement(By.CssSelector("a:nth-child(7) > img")).Click();
            driver.FindElement(By.Id("admin_passwd_trigger")).Click();
            driver.FindElement(By.Id("OldPassword")).SendKeys("ubnt");
            driver.FindElement(By.Id("NewPassword")).SendKeys(pass);
            driver.FindElement(By.Id("NewPassword2")).SendKeys(pass);
            driver.FindElement(By.Id("system_change")).Click();
            driver.FindElement(By.CssSelector("a:nth-child(3) > img")).Click();
            driver.FindElement(By.Id("wmode")).Click();
            {
                var dropdown = driver.FindElement(By.Id("wmode"));
                dropdown.FindElement(By.XPath("//option[. = 'Access Point']")).Click();
            }
            driver.FindElement(By.Id("wmode")).Click();
            driver.FindElement(By.Id("wds_chkbox")).Click();
            driver.FindElement(By.Id("hidessid_chk")).Click();
            driver.FindElement(By.Id("essid")).Click();
            driver.FindElement(By.Id("essid")).Clear();
            driver.FindElement(By.Id("essid")).SendKeys("ubnt1");
            driver.FindElement(By.Id("security")).Click();
            {
                var dropdown = driver.FindElement(By.Id("security"));
                dropdown.FindElement(By.XPath("//option[. = 'WPA2-AES']")).Click();
            }
            driver.FindElement(By.Id("wpa_key")).Click();
            driver.FindElement(By.Id("wpa_key")).SendKeys(pass);
            driver.FindElement(By.CssSelector(".change > input")).Click();
            driver.FindElement(By.CssSelector("a:nth-child(4) > img")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.Id("mgmtIpAddr")).Click();
            driver.FindElement(By.Id("mgmtIpAddr")).Clear();
            driver.FindElement(By.Id("mgmtIpAddr")).SendKeys(SelfIP);
            driver.FindElement(By.Id("mgmtIpNetmask")).Click();
            driver.FindElement(By.Id("mgmtIpNetmask")).Clear();
            driver.FindElement(By.Id("mgmtIpNetmask")).SendKeys(mask);
            driver.FindElement(By.Id("mgmtGateway")).Click();
            driver.FindElement(By.Id("mgmtGateway")).Clear();
            driver.FindElement(By.Id("mgmtGateway")).SendKeys(gateway);
            driver.FindElement(By.Id("change")).Click();
            driver.FindElement(By.CssSelector("a:nth-child(6) > img")).Click();
            driver.FindElement(By.Id("pwdogStatus")).Click();
            driver.FindElement(By.Id("pwdogHost")).Click();
            driver.FindElement(By.Id("pwdogHost")).SendKeys(CamIP);
            driver.FindElement(By.CssSelector(".change > input")).Click();
            driver.FindElement(By.Id("apply_button")).Click();
            Thread.Sleep(10000);
            driver.Close();
            TearDown();
    }
}
