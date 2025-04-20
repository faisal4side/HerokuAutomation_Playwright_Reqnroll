using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace HerokuAutomation_Playwright_Reqnroll.Pages
{
    public class TablePage : BasePage
    {
        public TablePage(IPage page) : base(page)
        {
        }

        public ILocator TableHeaders => Page.Locator("table#table1 th");
        public ILocator TableRows => Page.Locator("table#table1 tbody tr");
        public ILocator FirstNames => Page.Locator("table#table1 td:nth-child(2)");
        public ILocator LastNames => Page.Locator("table#table1 td:nth-child(1)");

        public async Task<List<string>> GetAllNames()
        {
            var firstNames = await FirstNames.AllTextContentsAsync();
            var lastNames = await LastNames.AllTextContentsAsync();
            
            return firstNames.Zip(lastNames, (first, last) => $"{first} {last}").ToList();
        }

        public async Task<List<Dictionary<string, string>>> GetTableData()
        {
            var headers = await TableHeaders.AllTextContentsAsync();
            var rows = await TableRows.AllAsync();
            
            var tableData = new List<Dictionary<string, string>>();
            
            foreach (var row in rows)
            {
                var cells = await row.Locator("td").AllTextContentsAsync();
                var rowData = new Dictionary<string, string>();
                
                for (int i = 0; i < headers.Count; i++)
                {
                    rowData[headers[i]] = cells[i];
                }
                
                tableData.Add(rowData);
            }
            
            return tableData;
        }

        public async Task<bool> IsNamePresentInTable(string fullName)
        {
            var nameParts = fullName.Split(' ');
            if (nameParts.Length != 2) return false;

            var tableData = await GetTableData();
            return tableData.Any(row => 
                row["First Name"] == nameParts[0] && 
                row["Last Name"] == nameParts[1]);
        }

        public async Task<bool> VerifyRowData(Dictionary<string, string> expectedData)
        {
            var tableData = await GetTableData();
            return tableData.Any(row =>
                expectedData.All(kvp => 
                    row.ContainsKey(kvp.Key) && 
                    row[kvp.Key] == kvp.Value));
        }
    }
} 