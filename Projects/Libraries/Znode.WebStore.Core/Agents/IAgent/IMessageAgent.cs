using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Agents
{
    public interface IMessageAgent
    {
        /// <summary>
        /// Get Message by key and area.
        /// </summary>
        /// <param name="key">Message Key.</param>
        /// <param name="area">Area.</param>
        /// <returns>Returns message against that key and area.</returns>
        string GetMessage(string key, string area);

        /// <summary>
        /// Get container based on the container Key
        /// </summary>
        /// <param name="containerKey">Container Key</param>
        /// <param name="area">Area</param>
        /// <returns>ContentContainerDataModel</returns>
        ContentContainerDataViewModel GetContainer(string containerKey);
    }
}
