namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Repositories
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces;
    using System.Data.Entity;
    using System.Threading.Tasks;
    public class TrackRepository : ITrackRepository
    {
        public async Task<Track> GetTrack(long id)
        {
            using (var context = new ChinookEntities())
            {
                return await context.Track.Include(x => x.Album).FirstOrDefaultAsync();
            }
        }

        public async Task<IODataQueryable<Track>> GetTracks(IODataQuery<Track> query)
        {
            using (var context = new ChinookEntities())
            {
                var r = query.ApplyTo(context.Track.Include(x => x.Album));

                return await r.ToListAsync();
            }
        }
    }
}