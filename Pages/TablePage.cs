using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HerokuAutomation_Playwright_Reqnroll.Pages
{
    public class TablePage : BasePage
    {
        public TablePage(IPage page) : base(page)
        {
        }

        public ILocator TableHeaders => Page.Locator("table#table1 th");
        public ILocator TableRows => Page.Locator("table#table1 tbody tr");
        public ILocator CompanyNames => Page.Locator("table#table1 td:nth-child(2)");

        public async Task<IReadOnlyList<string>> GetCompanyNames()
        {
            return await CompanyNames.AllTextContentsAsync();
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

        public async Task<bool> IsNamePresentInTable(string name)
        {
            return await Page.Locator($"table#table1 td:has-text('{name}')").CountAsync() > 0;
        }
    }
} 