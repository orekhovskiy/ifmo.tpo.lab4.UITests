//******************************************************************************
//
// Copyright (c) 2017 Microsoft Corporation. All rights reserved.
//
// This code is licensed under the MIT License (MIT).
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using System;
using System.Threading;

namespace ChatAppUITest
{
    public class ChatAppSession
    {
        private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string app = "C:/Users/anton/source/repos/ChatApp/ChatApp/bin/Debug/netcoreapp3.1/ChatApp.exe";

        protected static WindowsDriver<WindowsElement> session;
        protected static RemoteTouchScreen touchScreen;
        protected static WindowsDriver<WindowsElement> chatapp;
        protected static WindowsDriver<WindowsElement> mainwindowsession;

        public static void Setup(TestContext context)
        {
            if (session == null || touchScreen == null)
            {
                TearDown();

                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("app", "Root");
                session = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);
                Assert.IsNotNull(session);
                Assert.IsNotNull(session.SessionId);

                session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);

                touchScreen = new RemoteTouchScreen(session);
                Assert.IsNotNull(touchScreen);
            }
        }

        public static void TearDown()
        {
            touchScreen = null;

            if (session != null)
            {
                session.Close();
                session.Quit();
                session = null;
            }

            if (chatapp != null)
            {
                chatapp.Quit();
                chatapp = null;
            }

            if (mainwindowsession != null)
            {
                mainwindowsession.Quit();
                mainwindowsession = null;
            }
        }

        [TestInitialize]
        public virtual void TestInit()
        {
            WindowsElement title = null;

            try
            {
                DesiredCapabilities appCapabilities = new DesiredCapabilities();
                appCapabilities.SetCapability("app", app);
                chatapp = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), appCapabilities);


                var mainWindow = session.FindElementByName("MainWindow");
                var mainWindowHandle = mainWindow.GetAttribute("NativeWindowHandle");
                mainWindowHandle = (int.Parse(mainWindowHandle)).ToString("x");

                DesiredCapabilities mainCapabilities = new DesiredCapabilities();
                mainCapabilities.SetCapability("appTopLevelWindow", mainWindowHandle);

                mainwindowsession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), mainCapabilities);
                title = mainwindowsession.FindElementByAccessibilityId("MainWindowTitle");
            }
            catch
            {
                throw new Exception();
            }

            Assert.IsNotNull(title);
            Assert.IsTrue(title.Displayed);
        }
    }
}