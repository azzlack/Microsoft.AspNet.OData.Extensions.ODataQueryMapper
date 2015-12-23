namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Repositories
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class ArtistRepository : IArtistRepository
    {
        public async Task<IEnumerable<Artist>> GetArtists()
        {
            using (var context = new ChinookEntities())
            {
                return await context.Artist.ToListAsync();
            }
        }

        public async Task<IEnumerable<Artist>> GetArtists(IODataQuery<Artist> query)
        {
            using (var context = new ChinookEntities())
            {
                var r = query.ApplyTo(context.Artist);

                return await r.ToListAsync();
            }
        }
    }
}