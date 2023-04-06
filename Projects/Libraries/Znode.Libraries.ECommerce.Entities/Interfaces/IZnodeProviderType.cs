namespace Znode.Libraries.ECommerce.Entities
{
    /// <summary>
    /// This is the root interface for all provider types.
    /// </summary>
    public interface IZnodeProviderType
    {
        string ClassName { get; }
        string Name { get; set; }
        string Description { get; set; }
    }
}
