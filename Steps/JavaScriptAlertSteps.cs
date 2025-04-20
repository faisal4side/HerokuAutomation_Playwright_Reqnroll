using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;
using System.Threading.Tasks;
using HerokuAutomation_Playwright_Reqnroll.Pages;

namespace HerokuAutomation_Playwright_Reqnroll.Steps
{
    [Binding]
    public class JavaScriptAlertSteps
    {
        private readonly IPage _page;
        private readonly ScenarioContext _scenarioContext;
        private JavaScriptAlertsPage _jsAlertsPage;

        public JavaScriptAlertSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _page = scenarioContext.Get<IPage>("page");
            _jsAlertsPage = new JavaScriptAlertsPage(_page);
        }

        [Given(@"I am on the JavaScript alerts page")]
        public async Task GivenIAmOnTheJavaScriptAlertsPage()
        {
            await _page.GotoAsync("https://the-internet.herokuapp.com/javascript_alerts");
        }

        [When(@"I click on the JS Alert button")]
        public async Task WhenIClickOnTheJSAlertButton()
        {
            await _jsAlertsPage.ClickJsAlertButton();
        }

        [When(@"I click on the JS Confirm button")]
        public async Task WhenIClickOnTheJSConfirmButton()
        {
            await _jsAlertsPage.ClickJsConfirmButton();
        }

        [When(@"I click on the JS Prompt button")]
        public async Task WhenIClickOnTheJSPromptButton()
        {
            await _jsAlertsPage.ClickJsPromptButton("Test Message");
        }

        [Then(@"I should see the alert message ""(.*)""")]
        public async Task ThenIShouldSeeTheAlertMessage(string expectedMessage)
        {
            var result = await _jsAlertsPage.GetResultText();
            Assert.That(result, Does.Contain(expectedMessage));
        }

        [Then(@"I should accept the alert")]
        public void ThenIShouldAcceptTheAlert()
        {
            // Alert is already handled in the When step
        }

        [Then(@"I should accept the confirm")]
        public void ThenIShouldAcceptTheConfirm()
        {
            // Confirm is already handled in the When step
        }

        [Then(@"I should accept the prompt")]
        public void ThenIShouldAcceptThePrompt()
        {
            // Prompt is already handled in the When step
        }

        [Then(@"I should see the result containing ""(.*)""")]
        public async Task ThenIShouldSeeTheResultContaining(string expectedText)
        {
            var result = await _jsAlertsPage.GetResultText();
            Assert.That(result, Does.Contain(expectedText));
        }
    }
} 