using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;

namespace ChatAppUITest
{
    [TestClass]
    public class ScenarioChat : ChatAppSession
    {

        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string app = "C:/Users/anton/source/repos/ChatApp/ChatApp/bin/Debug/netcoreapp3.1/ChatApp.exe";
        private const string Login = "testuser";
        private const string Password = "testuser";

        private static WindowsDriver<WindowsElement> chat;

        [TestMethod]
        public void ShouldNotSendEmptyMessage()
        {
            chat.FindElementByAccessibilityId("ChatWindowSendMessageButton").Click();

            var messagebox = chat.FindElementByName("Send message fault");
            Assert.IsNotNull(messagebox);
            var text = messagebox.FindElementByName("Message can not be empty.");
            Assert.IsNotNull(text);
            messagebox.FindElementByName("Закрыть").Click();
        }

        [TestMethod]
        public void ShouldNotSendLongMessage()
        {
            chat.FindElementByAccessibilityId("ChatWindowMessageTextBox").SendKeys(new string('a', 257));
            chat.FindElementByAccessibilityId("ChatWindowSendMessageButton").Click();

            var messagebox = chat.FindElementByName("Send message fault");
            Assert.IsNotNull(messagebox);
            var text = messagebox.FindElementByName("Message length can not be greater than 256 characters.");
            Assert.IsNotNull(text);
            messagebox.FindElementByName("Закрыть").Click();
        }

        [TestMethod]
        public void ShouldNotSendDuplicateMessage()
        {
            chat.FindElementByAccessibilityId("ChatWindowMessageTextBox").SendKeys("test");
            chat.FindElementByAccessibilityId("ChatWindowSendMessageButton").Click();

            try
            {
                chat.FindElementByAccessibilityId("ChatWindowMessageTextBox").SendKeys("test");
                chat.FindElementByAccessibilityId("ChatWindowSendMessageButton").Click();
            }
            catch { }


            var messagebox = chat.FindElementByName("Send message fault");
            Assert.IsNotNull(messagebox);
            var text = messagebox.FindElementByName("Message should be different from your previous message.");
            Assert.IsNotNull(text);
            messagebox.FindElementByName("Закрыть").Click();
        }

        [TestMethod]
        public void ShouldUpdateMessageListOnMessageSend()
        {
            var messages = chat.FindElementByAccessibilityId("ScrollViewer")
                .FindElementByAccessibilityId("ChatWindowMessagesListBox");
            var messagessCount = messages.FindElements(By.Name("ChatApp.Models.Messages")).Count;

            var message = $"Current time is {DateTime.Now}";

            chat.FindElementByAccessibilityId("ChatWindowMessageTextBox").SendKeys(message);
            chat.FindElementByAccessibilityId("ChatWindowSendMessageButton").Click();

            messages = chat.FindElementByAccessibilityId("ScrollViewer")
                .FindElementByAccessibilityId("ChatWindowMessagesListBox");
            var newMessagesCount = messages.FindElements(By.Name("ChatApp.Models.Messages")).Count;

            Assert.AreEqual(messagessCount + 1, newMessagesCount);
        }

        [TestMethod]
        public void ShouldUpdateMessagesOnlyOnRefreshButtonClick()
        {
            var messages = chat.FindElementByAccessibilityId("ScrollViewer")
                .FindElementByAccessibilityId("ChatWindowMessagesListBox");
            var messagesCount = messages.FindElements(By.Name("ChatApp.Models.Messages")).Count;

            #region New chat user

            // new chat user
            DesiredCapabilities appCapabilities = new DesiredCapabilities();
            appCapabilities.SetCapability("app", app);
            var newchat = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);


            var mainWindow = session.FindElementByName("MainWindow");
            var mainWindowHandle = mainWindow.GetAttribute("NativeWindowHandle");
            mainWindowHandle = (int.Parse(mainWindowHandle)).ToString("x");

            DesiredCapabilities mainCapabilities = new DesiredCapabilities();
            mainCapabilities.SetCapability("appTopLevelWindow", mainWindowHandle);

            var newmainwindowsession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), mainCapabilities);

            // new user login process
            newmainwindowsession.FindElementByAccessibilityId("MainWindowLoginTextBox").SendKeys(Login);
            newmainwindowsession.FindElementByAccessibilityId("MainWindowPasswordPasswordBox").SendKeys(Password);
            newmainwindowsession.FindElementByAccessibilityId("MainWindowLoginButton").Click();

            var window = session.FindElementByName("Chat");
            var chatWindowHandle = window.GetAttribute("NativeWindowHandle");
            chatWindowHandle = (int.Parse(chatWindowHandle)).ToString("x");

            DesiredCapabilities capabilities = new DesiredCapabilities();
            capabilities.SetCapability("appTopLevelWindow", chatWindowHandle);

            var newchatuser = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), capabilities);

            #endregion 

            var message = $"Current time is {DateTime.Now}";

            newchatuser.FindElementByAccessibilityId("ChatWindowMessageTextBox").SendKeys(message);
            newchatuser.FindElementByAccessibilityId("ChatWindowSendMessageButton").Click();

            messages = chat.FindElementByAccessibilityId("ScrollViewer")
                .FindElementByAccessibilityId("ChatWindowMessagesListBox");
            var messagesAfterSendCount = messages.FindElements(By.Name("ChatApp.Models.Messages")).Count;

            Assert.AreEqual(messagesCount, messagesAfterSendCount);

            chat.FindElementByAccessibilityId("ChatWindowRefreshButton").Click();
            messages = chat.FindElementByAccessibilityId("ScrollViewer")
                .FindElementByAccessibilityId("ChatWindowMessagesListBox");
            var messagesAfterRefresh = messages.FindElements(By.Name("ChatApp.Models.Messages")).Count;
            Assert.AreEqual(messagesCount + 1, messagesAfterRefresh);
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
                chat.Close();
                chat.Quit();
                chat = null;
            }
            catch  { }
        }

        [TestInitialize]
        public override void TestInit()
        {
            base.TestInit();

            mainwindowsession.FindElementByAccessibilityId("MainWindowLoginTextBox").SendKeys(Login);
            mainwindowsession.FindElementByAccessibilityId("MainWindowPasswordPasswordBox").SendKeys(Password);
            mainwindowsession.FindElementByAccessibilityId("MainWindowLoginButton").Click();

            var window = session.FindElementByName("Chat");
            var chatWindowHandle = window.GetAttribute("NativeWindowHandle");
            chatWindowHandle = (int.Parse(chatWindowHandle)).ToString("x");

            DesiredCapabilities capabilities = new DesiredCapabilities();
            capabilities.SetCapability("appTopLevelWindow", chatWindowHandle);

            chat = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), capabilities);
        }

        #endregion
    }
}
