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

        [When(@"I extract all names from the table")]
        public async Task WhenIExtractAllNamesFromTheTable()
        {
            var names = await _tablePage.GetAllNames();
            _scenarioContext.Set(names, "Names");
        }

        [When(@"I extract data from the table")]
        public async Task WhenIExtractDataFromTheTable()
        {
            var tableData = await _tablePage.GetTableData();
            _scenarioContext.Set(tableData, "TableData");
        }

        [Then(@"I should see all names printed")]
        public void ThenIShouldSeeAllNamesPrinted()
        {
            var names = _scenarioContext.Get<List<string>>("Names");
            TestContext.WriteLine("All names in the table:");
            foreach (var name in names)
            {
                TestContext.WriteLine($"  {name}");
            }
        }

        [Then(@"I should verify that ""(.*)"" exists in the table")]
        public async Task ThenIShouldVerifyThatExistsInTheTable(string fullName)
        {
            var exists = await _tablePage.IsNamePresentInTable(fullName);
            Assert.That(exists, Is.True, $"Expected to find '{fullName}' in the table");
        }

        [Then(@"I should see the complete table data printed")]
        public void ThenIShouldSeeTheCompleteTableDataPrinted()
        {
            var tableData = _scenarioContext.Get<List<Dictionary<string, string>>>("TableData");
            TestContext.WriteLine("Complete table data:");
            foreach (var row in tableData)
            {
                TestContext.WriteLine("Row:");
                foreach (var kvp in row)
                {
                    TestContext.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }
                TestContext.WriteLine("");
            }
        }

        [Then(@"I should verify the following data exists in the table")]
        public async Task ThenIShouldVerifyTheFollowingDataExistsInTheTable(Table table)
        {
            var expectedData = table.Rows[0].ToDictionary(x => x.Key, x => x.Value);
            var exists = await _tablePage.VerifyRowData(expectedData);
            Assert.That(exists, Is.True, "Expected row data was not found in the table");
        }
    }
} 