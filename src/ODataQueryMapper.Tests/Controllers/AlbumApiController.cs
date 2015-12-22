namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces;

    public class AlbumApiController : ApiController
    {
        private readonly IAlbumFunctions albumFunctions;

        public AlbumApiController(IAlbumFunctions albumFunctions)
        {
            this.albumFunctions = albumFunctions;
        }

        [Route("api/album")]
        public async Task<HttpResponseMessage> Get()
        {
            var result = await this.albumFunctions.GetAlbums();

            if (result != null && result.Any())
            {
                return this.Request.CreateResponse(result);
            }

            return this.Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}