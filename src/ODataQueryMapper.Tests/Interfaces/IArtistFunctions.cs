namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.OData.Query;

    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;

    public interface IArtistFunctions
    {
        Task<IEnumerable<DomainArtist>> GetArtists();

        Task<IEnumerable<DomainArtist>> GetArtists(ODataQueryOptions<DomainArtist> query);
    }
}