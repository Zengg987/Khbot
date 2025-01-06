using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

//
// Backend for kahoot bot
//

namespace Kahoot_Bot
{
    public class Host
    {
        public string? lobbyID;
        public string botName;
        public static IWebDriver? driver; // webdriver for browser control

        internal Bot[] bots; // array of all bots in game
        private static ChromeOptions? options;
        private static ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
        public Host(string ID, string name, int botCount)
        {
            lobbyID = ID;
            botName = name;
            bots = new Bot[botCount];
        }

        public void Initialise_Webdriver()
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            options = new ChromeOptions();
            options.AddArgument("headless");
            driverService.HideCommandPromptWindow = true;
            try
            {
                driver = new ChromeDriver(driverService, options);
            }
            catch (NoSuchElementException)
            {
                throw new NoSuchElementException("Web driver failed. Try adding chromedriver to path");
            }
        }

        public bool Join_Game(int botNumber, bool delay)
        {
            const string GAME_URL = "https://kahoot.it/";
            string numberedBotName = botName + botNumber; // Bot name with individual number
            bool joinSuccessful = false;
            var bot = new Bot { Status = "Failed" };

            if (driver is null)
            {
                Console.WriteLine("Webdriver is null");
                return false;
            }

            int maxRetries = 3; // Number of retry attempts
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    driver.Navigate().GoToUrl(GAME_URL);

                    if (delay)
                    {
                        // Introduce a random delay to mimic human behavior
                        Random random = new Random();
                        int delayTime = random.Next(300, 1200); // Randomized delay between 300ms and 1200ms
                        Thread.Sleep(delayTime);
                    }

                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                    // Enter lobby ID
                    var gamePinTextBox = wait.Until(e => e.FindElement(By.Id("game-input")));
                    gamePinTextBox.Clear();
                    gamePinTextBox.SendKeys(lobbyID + Keys.Enter);

                    // Enter nickname
                    var nicknameTextBox = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("nickname")));
                    nicknameTextBox.Clear();
                    nicknameTextBox.SendKeys(numberedBotName + Keys.Enter);

                    // Verify successful join (check if redirected to the game screen)
                    if (wait.Until(e => e.Url.Contains("https://kahoot.it/instructions"))) 
                    {
                        joinSuccessful = true;
                        bot.joinSuccessful = true;
                        bot.Status = "Success";
                        bot.Name = numberedBotName;
                        break; // Exit the retry loop if successful
                    }
                }
                catch (WebDriverTimeoutException ex)
                {
                    Console.WriteLine($"Timeout occurred on attempt {attempt}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error on attempt {attempt}: {ex.Message}");
                }

                Console.WriteLine($"Join attempt {attempt} failed for bot {numberedBotName}. Retrying...");
            }

            if (!joinSuccessful)
            {
                Console.WriteLine($"Bot {numberedBotName} failed to join after {maxRetries} attempts.");
                bot.Status = "Failed after retries";
            }

            bots[botNumber] = bot; 
            return joinSuccessful;
        }



        public List<string> Remove_Options()
        {
            var buttonClasses = new List<string> { "answer-0", "answer-1", "answer-2", "answer-3" }; // html classes of the 4 kahoot buttons
            var tmp = new List<string> { "answer-0", "answer-1", "answer-2", "answer-3" };

            if (driver is null)
            {
                throw new NullReferenceException();
            }
            // you cannot remove items from a array whilst enumerating through it so two arrays are needed

            foreach (var className in buttonClasses)
            {
                try
                {
                    driver.FindElement(By.ClassName(className));
                }
                catch (NoSuchElementException)
                {
                    tmp.Remove(className);
                }
            }

            buttonClasses = tmp;
            return buttonClasses;
        }

        public void Answer_Question(List<string> availableAnswers)
        {
            // get a bot to randomly answer a kahoot question
            var random = new Random();
            int numberOfButtons = availableAnswers.Count;
            int index = random.Next(0, numberOfButtons);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            try
            {
                // wait until button is available
                var button = wait.Until(e => e.FindElement(By.ClassName(availableAnswers[index])));
                button.Click();
            }
            catch (TimeoutException)
            {
                Debug.WriteLine("Failed to answer");
            }
        }

        public int Scrape_Score()
        {
            // get current score for a single bot
            var wait = new WebDriverWait(Host.driver, new TimeSpan(0, 0, 3));
            int score = 0;
            try
            {
                const string SCORE_CLASS = "hycjox";
                var scoreElement = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(SCORE_CLASS)));
                score = int.Parse(scoreElement.Text);

            }
            catch (WebDriverTimeoutException)
            {
                Debug.WriteLine("Failed to scrape score");
            }
            return score;
        }

        public void Wait_For_URL_Change()
        {
            // pause until page changes to a new URL
            // example wait until the lobby page changes into the page of the first question 
            if (driver is null)
            {
                throw new NullReferenceException();
            }
            string currentURL = driver.Url;
            while (currentURL == driver.Url) { }
        }

        public void Shutdown_Host()
        {
            // end host and shutdown chromedriver
            if (driver is null)
            {
                throw new NullReferenceException();
            }
            driver.Quit();
        }
    }
}