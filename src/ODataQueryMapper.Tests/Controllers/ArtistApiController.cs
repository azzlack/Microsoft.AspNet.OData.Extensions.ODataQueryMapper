namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces;

    public class ArtistApiController : ApiController
    {
        private readonly IArtistFunctions artistFunctions;

        public ArtistApiController(IArtistFunctions artistFunctions)
        {
            this.artistFunctions = artistFunctions;
        }

        [Route("api/artist")]
        public async Task<HttpResponseMessage> Get()
        {
            var result = await this.artistFunctions.GetArtists();

            if (result != null && result.Any())
            {
                return this.Request.CreateResponse(result);
            }

            return this.Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}