using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HerokuAutomation_Playwright_Reqnroll.Pages;

namespace HerokuAutomation_Playwright_Reqnroll.Steps
{
    [Binding]
    public class TableHandlingSteps
    {
        private readonly IPage _page;
        private readonly ScenarioContext _scenarioContext;
        private TablePage _tablePage;

        public TableHandlingSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _page = scenarioContext.Get<IPage>("page");
            _tablePage = new TablePage(_page);
        }

        [Given(@"I am on the tables page")]
        public async Task GivenIAmOnTheTablesPage()
        {
            await _page.GotoAsync("https://the-internet.herokuapp.com/tables");
        }

        [When(@"I extract all company names from the table")]
        public async Task WhenIExtractAllCompanyNamesFromTheTable()
        {
            var companyNames = await _tablePage.GetCompanyNames();
            _scenarioContext.Set(companyNames.ToList(), "CompanyNames");
        }

        [When(@"I extract data from the table with headers")]
        public async Task WhenIExtractDataFromTheTableWithHeaders()
        {
            var tableData = await _tablePage.GetTableData();
            _scenarioContext.Set(tableData, "TableData");
        }

        [Then(@"I should see the company names printed")]
        public void ThenIShouldSeeTheCompanyNamesPrinted()
        {
            var companyNames = _scenarioContext.Get<List<string>>("CompanyNames");
            foreach (var name in companyNames)
            {
                TestContext.WriteLine($"Company Name: {name}");
            }
        }

        [Then(@"I should verify that ""(.*)"" exists in the table")]
        public async Task ThenIShouldVerifyThatExistsInTheTable(string name)
        {
            var nameExists = await _tablePage.IsNamePresentInTable(name);
            Assert.That(nameExists, Is.True, $"Expected to find '{name}' in the table");
        }

        [Then(@"I should see the table data printed")]
        public void ThenIShouldSeeTheTableDataPrinted()
        {
            var tableData = _scenarioContext.Get<List<Dictionary<string, string>>>("TableData");
            foreach (var row in tableData)
            {
                TestContext.WriteLine("Row Data:");
                foreach (var kvp in row)
                {
                    TestContext.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }
                TestContext.WriteLine("");
            }
        }
    }
} 