namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.OData.Query;

    public interface IArtistRepository
    {
        Task<IEnumerable<Artist>> GetArtists();

        Task<IEnumerable<Artist>> GetArtists(ODataQueryOptions<Artist> query);
    }
}