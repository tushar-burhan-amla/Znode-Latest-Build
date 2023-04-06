namespace Znode.Libraries.Abstract.Configuration
{
	public class CacheRoute
	{
		public bool Enabled { get; set; }
		public int Duration { get; set; }
		public bool Sliding { get; set; }
		public string Template { get; set; }
	}
}