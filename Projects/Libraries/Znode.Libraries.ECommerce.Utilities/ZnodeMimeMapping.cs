using System.Web;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class ZnodeMimeMapping
    {
        // Get the content type header of the file. Example: fileName format as "0bcab774-8ca0-4a72-ab9f-afd56cf87d8abroccoli.jpg".
        public static string GetMimeMapping(string fileName)
          => MimeMapping.GetMimeMapping(fileName);
    }
}
