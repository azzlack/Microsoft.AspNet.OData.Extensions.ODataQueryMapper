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

    [ValidateQuery(AllowedQueryOptions = AllowedQueryOptions.Filter | AllowedQueryOptions.Count | AllowedQueryOptions.OrderBy | AllowedQueryOptions.Skip | AllowedQueryOptions.Top)]
    public class TrackODataController : ODataController
    {
        private readonly ITrackFunctions trackFunctions;

        public TrackODataController(ITrackFunctions trackFunctions)
        {
            this.trackFunctions = trackFunctions;
        }

        [ODataRoute("track")]
        public async Task<IHttpActionResult> Get(ODataQueryOptions<DomainTrack> query)
        {
            var result = await this.trackFunctions.GetTracks(query);

            if (result != null && result.Any())
            {
                return this.Ok(result);
            }

            return this.StatusCode(HttpStatusCode.NoContent);
        }

        [ODataRoute("track({id})")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var result = await this.trackFunctions.GetTrack(id);

            if (result != null)
            {
                return this.Ok(result);
            }

            return this.StatusCode(HttpStatusCode.NotFound);
        }
    }
}