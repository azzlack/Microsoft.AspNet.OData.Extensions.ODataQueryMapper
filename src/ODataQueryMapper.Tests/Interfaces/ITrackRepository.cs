namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using System.Threading.Tasks;

    public interface ITrackRepository
    {
        Task<Track> GetTrack(long id);

        Task<IODataQueryable<Track>> GetTracks(IODataQuery<Track> query);
    }
}