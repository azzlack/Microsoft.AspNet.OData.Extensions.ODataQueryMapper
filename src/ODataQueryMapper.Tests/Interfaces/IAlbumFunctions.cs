namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.OData.Query;

    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;

    public interface IAlbumFunctions
    {
        Task<IEnumerable<DomainAlbum>> GetAlbums();

        Task<IEnumerable<DomainAlbum>> GetAlbums(ODataQueryOptions<DomainAlbum> query);
    }
}