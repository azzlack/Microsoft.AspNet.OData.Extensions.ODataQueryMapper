namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Functions
{
    using AutoMapper;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.OData.Query;

    public class AlbumFunctions : IAlbumFunctions
    {
        private readonly IAlbumRepository albumRepository;

        private readonly IMappingEngine mapper;

        private readonly IODataQueryMapper odataMapper;

        public AlbumFunctions(IAlbumRepository albumRepository, IMappingEngine mapper, IODataQueryMapper odataMapper)
        {
            this.albumRepository = albumRepository;
            this.mapper = mapper;
            this.odataMapper = odataMapper;
        }

        public async Task<IEnumerable<DomainAlbum>> GetAlbums()
        {
            var albums = await this.albumRepository.GetAlbums();

            return this.mapper.Map<IEnumerable<DomainAlbum>>(albums);
        }

        public async Task<IEnumerable<DomainAlbum>> GetAlbums(ODataQueryOptions<DomainAlbum> query)
        {
            var q = this.odataMapper.Map<DomainAlbum, Album>(query);

            var albums = await this.albumRepository.GetAlbums(q);

            return this.mapper.Map<IEnumerable<DomainAlbum>>(albums);
        }
    }
}