using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;

namespace ChatAppUITest
{
    [TestClass]
    public class ScenarioRegister : ChatAppSession
    {

        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string Login = "testuser";
        private const string Password = "testuser";
        private const string Firstname = "testuser";
        private const string Lastname = "testuser";

        private static WindowsDriver<WindowsElement> register;

        [TestMethod]
        public void ShouldNotRegisterWithLongLogin()
        {
            register.FindElementByAccessibilityId("RegisterWindowLoginTextBox").SendKeys(new string('a', 16));
            register.FindElementByAccessibilityId("RegisterWindowRegisterButton").Click();
        }

        [TestMethod]
        public void ShouldNotRegisterWithShortLogin()
        {
            register.FindElementByAccessibilityId("RegisterWindowLoginTextBox").SendKeys(new string('a', 5));
            register.FindElementByAccessibilityId("RegisterWindowRegisterButton").Click();
        }

        [TestMethod]
        public void ShouldNotRegisterWitInvalidLogin()
        {
            register.FindElementByAccessibilityId("RegisterWindowLoginTextBox").SendKeys("123456");
            register.FindElementByAccessibilityId("RegisterWindowRegisterButton").Click();
        }

        [TestMethod]
        public void ShouldNotRegisterWitLongPassword()
        {
            register.FindElementByAccessibilityId("RegisterWindowLoginTextBox").SendKeys(Login);
            register.FindElementByAccessibilityId("RegisterWindowPasswordPasswordBox").SendKeys(new string('a', 16));
            register.FindElementByAccessibilityId("RegisterWindowRegisterButton").Click();
        }

        [TestMethod]
        public void ShouldNotRegisterWitShortPassword()
        {
            register.FindElementByAccessibilityId("RegisterWindowLoginTextBox").SendKeys(Login);
            register.FindElementByAccessibilityId("RegisterWindowPasswordPasswordBox").SendKeys(new string('a', 5));
            register.FindElementByAccessibilityId("RegisterWindowRegisterButton").Click();
        }

        [TestMethod]
        public void ShouldNotRegisterWitLongFirstname()
        {
            register.FindElementByAccessibilityId("RegisterWindowLoginTextBox").SendKeys(Login);
            register.FindElementByAccessibilityId("RegisterWindowPasswordPasswordBox").SendKeys(Password);
            register.FindElementByAccessibilityId("RegisterWindowFirstnameTextBox").SendKeys(new string('a', 31));
            register.FindElementByAccessibilityId("RegisterWindowRegisterButton").Click();
        }

        [TestMethod]
        public void ShouldNotRegisterWitEmptyFirstname()
        {
            register.FindElementByAccessibilityId("RegisterWindowLoginTextBox").SendKeys(Login);
            register.FindElementByAccessibilityId("RegisterWindowPasswordPasswordBox").SendKeys(Password);
            register.FindElementByAccessibilityId("RegisterWindowRegisterButton").Click();
        }

        [TestMethod]
        public void ShouldNotRegisterWitLongLastname()
        {
            register.FindElementByAccessibilityId("RegisterWindowLoginTextBox").SendKeys(Login);
            register.FindElementByAccessibilityId("RegisterWindowPasswordPasswordBox").SendKeys(Password);
            register.FindElementByAccessibilityId("RegisterWindowFirstnameTextBox").SendKeys(Firstname);
            register.FindElementByAccessibilityId("RegisterWindowLastnameTextBox").SendKeys(new string('a', 31));
            register.FindElementByAccessibilityId("RegisterWindowRegisterButton").Click();
        }

        [TestMethod]
        public void ShouldNotRegisterWitEmptyLastname()
        {
            register.FindElementByAccessibilityId("RegisterWindowLoginTextBox").SendKeys(Login);
            register.FindElementByAccessibilityId("RegisterWindowPasswordPasswordBox").SendKeys(Password);
            register.FindElementByAccessibilityId("RegisterWindowFirstnameTextBox").SendKeys(Firstname);
            register.FindElementByAccessibilityId("RegisterWindowRegisterButton").Click();
        }

        [TestMethod] public void ShouldNotRegisterWitDuplicateLogin()
        {
            register.FindElementByAccessibilityId("RegisterWindowLoginTextBox").SendKeys(Login);
            register.FindElementByAccessibilityId("RegisterWindowPasswordPasswordBox").SendKeys(Password);
            register.FindElementByAccessibilityId("RegisterWindowLastnameTextBox").SendKeys(Lastname);
            register.FindElementByAccessibilityId("RegisterWindowFirstnameTextBox").SendKeys(Firstname);
            register.FindElementByAccessibilityId("RegisterWindowRegisterButton").Click();
        }

        #region Automation things

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            try
            {
                register.FindElementByName("Registration fault").FindElementByName("Закрыть").Click();
                register.Close();
            }
            catch
            {

            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            try
            {
                register.Close();
                register.Quit();
                register = null;
            }
            catch
            {

            }
        }

        [TestInitialize]
        public override void TestInit()
        {
            base.TestInit();

            mainwindowsession.FindElementByAccessibilityId("MainWindowRegisterButton").Click();

            var window = session.FindElementByName("Register");
            var chatWindowHandle = window.GetAttribute("NativeWindowHandle");
            chatWindowHandle = (int.Parse(chatWindowHandle)).ToString("x");

            DesiredCapabilities capabilities = new DesiredCapabilities();
            capabilities.SetCapability("appTopLevelWindow", chatWindowHandle);

            register = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), capabilities);
        }

        #endregion
    }
}
