namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Repositories
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class AlbumRepository : IAlbumRepository
    {
        public async Task<IEnumerable<Album>> GetAlbums()
        {
            using (var context = new ChinookEntities())
            {
                return await context.Album.Include(x => x.Artist).ToListAsync();
            }
        }

        public async Task<IODataQueryable<Album>> GetAlbums(IODataQuery<Album> query)
        {
            using (var context = new ChinookEntities())
            {
                var r = query.ApplyTo(context.Album.Include(x => x.Artist));

                return await r.ToListAsync();
            }
        }
    }
}