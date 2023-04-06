namespace Znode.Engine.QuickBook
{
    /// <summary>
    /// Model required for QuickBook XML add customer request node element.
    /// BaseModel is having model with property full name
    /// </summary>
    public class CustomerAddRq : BaseModel
    {
        public CustomerAdd CustomerAdd { get; set; }
    }
}