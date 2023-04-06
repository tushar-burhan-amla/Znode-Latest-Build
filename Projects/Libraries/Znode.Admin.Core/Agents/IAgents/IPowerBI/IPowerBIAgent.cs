using System.Threading.Tasks;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IPowerBIAgent
    {
        /// <summary>
        /// Method to get the Power BI embed details
        /// </summary>
        /// <parameter>PowerBiSettingsViewModel</parameter>
        /// <returns>return response of power bi</returns>
        Task<PowerBIEmbedViewModel> GetPowerBIReportsData(PowerBISettingsViewModel powerBISettingsViewModel);
    }
}
