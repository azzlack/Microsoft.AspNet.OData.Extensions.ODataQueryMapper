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

    [ODataRoutePrefix("artist")]
    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, PageSize = 50)]
    public class ArtistODataController : ODataController
    {
        private readonly IArtistFunctions artistFunctions;

        public ArtistODataController(IArtistFunctions artistFunctions)
        {
            this.artistFunctions = artistFunctions;
        }

        [ODataRoute]
        public async Task<IHttpActionResult> Get(ODataQueryOptions<DomainArtist> query)
        {
            var result = await this.artistFunctions.GetArtists(query);

            if (result != null && result.Any())
            {
                return this.Ok(result);
            }

            return this.StatusCode(HttpStatusCode.NoContent);
        }
    }
}