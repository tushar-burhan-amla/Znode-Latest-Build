namespace Znode.Libraries.ECommerce.Utilities
{
    public class CacheRoute
	{
		public bool Enabled { get; set; }
		public int Duration { get; set; }
		public bool Sliding { get; set; }
		public string Template { get; set; }
        public string Key { get; set; } = string.Empty;
    }
}