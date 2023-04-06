using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Agents
{
    public interface ICaseRequestAgent
    {
        /// <summary>
        /// Create CaseRequest.
        /// </summary>
        /// <param name="CaseRequestViewModel">CaseRequest View Model.</param>
        /// <returns>Returns created model.</returns>
        CaseRequestViewModel CreateContactUs(CaseRequestViewModel CaseRequestViewModel);

        /// <summary>
        /// Create customer feedback form.
        /// </summary>
        /// <param name="caseRequestViewModel"></param>
        /// <returns></returns>
        CaseRequestViewModel CreateCustomerFeedback(CaseRequestViewModel caseRequestViewModel);
    }
}
