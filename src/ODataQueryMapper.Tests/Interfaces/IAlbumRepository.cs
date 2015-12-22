namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAlbumRepository
    {
        Task<IEnumerable<Album>> GetAlbums();

        Task<IEnumerable<Album>> GetAlbums(IODataQuery<Album> query);
    }
}