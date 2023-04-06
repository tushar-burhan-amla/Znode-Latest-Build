using Newtonsoft.Json;
using System.Collections.Generic;

namespace Znode.Libraries.Cloudflare.API
{
    public class FilesListModel
    {
        [JsonProperty("files")]
        public List<string> Files { get; set; }
    }
}
