using System.Threading.Tasks;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Agents
{
    public interface IPowerBIAgent
    {
        /// <summary>
        /// Method to get the Power BI Report embed details
        /// </summary>
        /// <parameter>PowerBiSettingsViewModel</parameter>
        /// <returns>return the response of Power BI Report as PowerBIEmbedViewModel</returns>
        Task<PowerBIEmbedViewModel> GetPowerBIReportsData(PowerBISettingsViewModel powerBISettingsViewModel);

        /// <summary>
        /// Get the Power BI settings details
        /// </summary>
        /// <returns>returns Power BI settings details as PowerBISettingsViewModel</returns>
        PowerBISettingsViewModel GetPowerBISettings();
    }
}