namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Controllers
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.OData;
    using System.Web.OData.Query;
    using System.Web.OData.Routing;

    [ODataRoutePrefix("album")]
    [ValidateQuery(AllowedQueryOptions = AllowedQueryOptions.Filter | AllowedQueryOptions.Count | AllowedQueryOptions.OrderBy | AllowedQueryOptions.Skip | AllowedQueryOptions.Top, PageSize = 50)]
    public class AlbumODataController : ODataController
    {
        private readonly IAlbumFunctions albumFunctions;

        private string cached;

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
                this.cached = JsonConvert.SerializeObject(result);

                return this.Ok(result);
            }

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        [ODataRoute]
        public async Task<IHttpActionResult> Get(int key, ODataQueryOptions<DomainAlbum> query)
        {
            var result = JsonConvert.DeserializeObject<IODataQueryable<DomainAlbum>>(this.cached);

            if (result != null && result.Any())
            {
                return this.Ok(result);
            }

            return this.StatusCode(HttpStatusCode.NoContent);
        }
    }
}