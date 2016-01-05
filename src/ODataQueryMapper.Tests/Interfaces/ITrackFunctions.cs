namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.OData.Query;

    public interface ITrackFunctions
    {
        Task<DomainTrack> GetTrack(int id);

        Task<IEnumerable<DomainTrack>> GetAlbumTracks(long albumId);

        Task<IEnumerable<DomainTrack>> GetTracks(ODataQueryOptions<DomainTrack> query);
    }
}