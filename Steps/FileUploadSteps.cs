using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;
using System.Threading.Tasks;
using HerokuAutomation_Playwright_Reqnroll.Pages;

namespace HerokuAutomation_Playwright_Reqnroll.Steps
{
    [Binding]
    public class FileUploadSteps
    {
        private readonly IPage _page;
        private readonly ScenarioContext _scenarioContext;
        private FileUploadPage _fileUploadPage;

        public FileUploadSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _page = scenarioContext.Get<IPage>();
            _fileUploadPage = new FileUploadPage(_page);
        }

        [Given(@"I am on the file upload page")]
        public async Task GivenIAmOnTheFileUploadPage()
        {
            await _page.GotoAsync("https://the-internet.herokuapp.com/upload");
        }

        [When(@"I upload the file ""(.*)""")]
        public async Task WhenIUploadTheFile(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestData", fileName);
            await _fileUploadPage.UploadFile(filePath);
        }

        [Then(@"I should see the uploaded file name ""(.*)""")]
        public async Task ThenIShouldSeeTheUploadedFileName(string fileName)
        {
            var uploadedFileName = await _fileUploadPage.GetUploadedFileName();
            Assert.That(uploadedFileName, Is.EqualTo(fileName));
        }

        [Then(@"I should see the upload success message")]
        public async Task ThenIShouldSeeTheUploadSuccessMessage()
        {
            var successMessage = await _fileUploadPage.GetSuccessMessage();
            Assert.That(successMessage, Is.EqualTo("File Uploaded!"));
        }
    }
} 