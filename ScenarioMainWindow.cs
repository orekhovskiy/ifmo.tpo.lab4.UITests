using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using System.Threading;
using System;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace ChatAppUITest
{
    [TestClass]
    public class ScenarioMainWindow : ChatAppSession
    {
        private const string Login = "testuser";
        private const string Password = "testuser";
        private const string WrongPassword = "wrongpassword";
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";

        private static WindowsDriver<WindowsElement> chat;
        private static WindowsDriver<WindowsElement> register;

        [TestMethod]
        public void ShouldLogin()
        {
            mainwindowsession.FindElementByAccessibilityId("MainWindowLoginTextBox").Clear();
            mainwindowsession.FindElementByAccessibilityId("MainWindowLoginTextBox").SendKeys(Login);

            mainwindowsession.FindElementByAccessibilityId("MainWindowPasswordPasswordBox").Clear();
            mainwindowsession.FindElementByAccessibilityId("MainWindowPasswordPasswordBox").SendKeys(Password);

            mainwindowsession.FindElementByAccessibilityId("MainWindowLoginButton").Click();
            
            Thread.Sleep(1000);

            ChatSetUp();
            var currentLogin = chat.FindElementByAccessibilityId("ChatWindowCurrentLoginTextBlock");
            Assert.IsNotNull(currentLogin);
            Assert.AreEqual(Login, currentLogin.Text);
        }

        [TestMethod]
        public void ShouldNotLogin()
        {
            mainwindowsession.FindElementByAccessibilityId("MainWindowLoginTextBox").Clear();
            mainwindowsession.FindElementByAccessibilityId("MainWindowLoginTextBox").SendKeys(Login);

            mainwindowsession.FindElementByAccessibilityId("MainWindowPasswordPasswordBox").Clear();
            mainwindowsession.FindElementByAccessibilityId("MainWindowPasswordPasswordBox").SendKeys(WrongPassword);

            mainwindowsession.FindElementByAccessibilityId("MainWindowLoginButton").Click();

            Thread.Sleep(2000);
            var messagebox = mainwindowsession.FindElementByName("Login fault");
            Assert.IsNotNull(messagebox);
            var text = messagebox.FindElementByName("Authorization failed. Please check the login and password.");
            Assert.IsNotNull(text);
            messagebox.FindElementByName("Закрыть").Click();
        }

        [TestMethod]
        public void ShouldSwitchToRegister()
        {
            mainwindowsession.FindElementByAccessibilityId("MainWindowRegisterButton").Click();
            RegisterSetUp();

            var title = register.FindElementByAccessibilityId("RegisterWindowTitle");
            Assert.IsNotNull(title);
        }

        #region Test automating things

        private static void RegisterSetUp()
        {
            var window = session.FindElementByName("Register");
            var chatWindowHandle = window.GetAttribute("NativeWindowHandle");
            chatWindowHandle = (int.Parse(chatWindowHandle)).ToString("x");

            DesiredCapabilities capabilities = new DesiredCapabilities();
            capabilities.SetCapability("appTopLevelWindow", chatWindowHandle);

            register = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), capabilities);
        }

        private static void ChatSetUp()
        {
            var chatWindow = session.FindElementByName("Chat");
            var chatWindowHandle = chatWindow.GetAttribute("NativeWindowHandle");
            chatWindowHandle = (int.Parse(chatWindowHandle)).ToString("x");

            DesiredCapabilities capabilities = new DesiredCapabilities();
            capabilities.SetCapability("appTopLevelWindow", chatWindowHandle);

            chat = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), capabilities);
        }


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            try
            {
                mainwindowsession.Close();
                mainwindowsession.Quit();
                mainwindowsession.Close();
            } catch { }
            try
            {
                chatapp.Close();
                chatapp.Quit();
                chatapp = null;
            } catch { }
            try
            {
                register.Close();
                register.Quit();
                register = null;
            } catch { }
            try
            {
                chat.Close();
                chat.Quit();
                chat = null;
            } catch { }

            TearDown();
        }

        [TestInitialize]
        public override void TestInit()
        {
            base.TestInit();
        }



        #endregion
    }
}
