namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IArtistRepository
    {
        Task<IEnumerable<Artist>> GetArtists();

        Task<IEnumerable<Artist>> GetArtists(IODataQuery<Artist> query);
    }
}