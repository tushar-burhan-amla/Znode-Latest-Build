using Znode.WebStore.Core.ViewModels;

namespace Znode.WebStore.Core.Agents
{
    public interface ISearchReportAgent
    {
        /// <summary>
        /// Save Search Report Data.
        /// </summary>
        /// <param name="viewModel">Search Report Data to be save in DB.</param>
        /// <returns>Inserted Data from The Db.</returns>
        SearchReportViewModel SaveSearchReport(SearchReportViewModel viewModel);
    }
}
