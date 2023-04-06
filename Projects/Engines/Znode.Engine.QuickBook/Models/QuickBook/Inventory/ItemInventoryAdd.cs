namespace Znode.Engine.QuickBook
{
    /// <summary>
    /// Model required for QuickBook XML add inventory item node element
    /// </summary>
    public class ItemInventoryAdd
    {
        public string Name { get; set; }

        /// <summary>
        /// It only accepts characters numbers.
        /// </summary>
        public string SalesDesc { get; set; }

        public string SalesPrice { get; set; }
        public Ref IncomeAccountRef { get; set; }
        public Ref COGSAccountRef { get; set; }
        public Ref AssetAccountRef { get; set; }
    }
}