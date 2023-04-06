using System.Net.Http;
using System.Web.Mvc;
using Znode.Engine.Api.Controllers;

namespace Znode.Engine.Api.Areas.Ecommerce.Controllers
{
    public class CartController : BaseController
    {
        // GET: Ecommerce/Cart
        [HttpGet]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response = null;
            return response;
        }
    }
}