namespace Znode.Libraries.Paypal
{
    /// <summary>
    /// Paypal Recurring Billing Info
    /// </summary>
    public class PaypalRecurringBillingInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether installment indicator is on or off
        /// </summary>        
        public bool InstallmentInd { get; set; }


        /// <summary>
        /// Gets or sets the period
        /// </summary>        
        public string Period { get; set; }

        /// <summary>
        /// Gets or sets the frequency
        /// </summary>        
        public string Frequency { get; set; }

        /// <summary>
        /// Gets or sets the total cycles
        /// </summary>        
        public int TotalCycles { get; set; }

        /// <summary>
        /// Gets or sets the initial amount
        /// </summary>        
        public decimal InitialAmount { get; set; }

        /// <summary>
        /// Gets or sets the amount
        /// </summary>        
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the profile name
        /// </summary>        
        public string ProfileName { get; set; }
    }
}