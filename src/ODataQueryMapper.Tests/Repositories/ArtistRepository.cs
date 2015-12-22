namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Repositories
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.OData.Query;

    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces;

    public class ArtistRepository : IArtistRepository
    {
        public async Task<IEnumerable<Artist>> GetArtists()
        {
            using (var context = new ChinookContext())
            {
                return await context.Artist.ToListAsync();
            }
        }

        public async Task<IEnumerable<Artist>> GetArtists(ODataQueryOptions<Artist> query)
        {
            using (var context = new ChinookContext())
            {
                var r = query.ApplyTo(context.Artist) as IQueryable<Artist>;

                return await r.ToListAsync();
            }
        }
    }
}