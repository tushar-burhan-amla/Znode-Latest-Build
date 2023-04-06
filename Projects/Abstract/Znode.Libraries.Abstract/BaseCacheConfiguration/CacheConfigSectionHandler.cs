using System;
using System.Configuration;
using System.Xml;

namespace Znode.Libraries.Abstract.Configuration
{
	public class CacheConfigSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			var cacheConfig = new CacheConfiguration();

			var cacheNode = section.SelectSingleNode("cache");
			if (cacheNode == null || cacheNode.Attributes == null)
			{
				cacheConfig.Enabled = false;
			}
			else
			{
				cacheConfig.Enabled = Convert.ToBoolean(cacheNode.Attributes["enabled"].Value);

				var routeNodes = section.SelectNodes("cache/routes/route");
				if (routeNodes != null)
				{
					foreach (XmlElement element in routeNodes)
					{
						var cacheRoute = new CacheRoute
						{
							Duration = Convert.ToInt32(element.GetAttribute("duration")),
							Enabled = Convert.ToBoolean(element.GetAttribute("enabled")),
							Sliding = Convert.ToBoolean(element.GetAttribute("sliding")),
							Template = element.GetAttribute("template")
						};

						cacheConfig.CacheRoutes.Add(cacheRoute);
					}
				}
			}
            
			return cacheConfig;
		}
	}
}
