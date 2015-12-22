namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Controllers
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.OData;
    using System.Web.OData.Query;
    using System.Web.OData.Routing;

    [ODataRoutePrefix("album")]
    [EnableQuery]
    public class AlbumODataController : ODataController
    {
        private readonly IAlbumFunctions albumFunctions;

        public AlbumODataController(IAlbumFunctions albumFunctions)
        {
            this.albumFunctions = albumFunctions;
        }

        [ODataRoute]
        public async Task<IHttpActionResult> Get(ODataQueryOptions<DomainAlbum> query)
        {
            var result = await this.albumFunctions.GetAlbums(query);

            if (result != null && result.Any())
            {
                return this.Ok(result);
            }

            return this.StatusCode(HttpStatusCode.NoContent);
        }
    }
}