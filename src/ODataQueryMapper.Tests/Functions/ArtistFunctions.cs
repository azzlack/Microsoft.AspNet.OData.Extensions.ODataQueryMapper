namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Functions
{
    using AutoMapper;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.OData.Query;

    public class ArtistFunctions : IArtistFunctions
    {
        private readonly IArtistRepository artistRepository;

        private readonly IMappingEngine mapper;

        private readonly IODataQueryMapper odataMapper;

        public ArtistFunctions(IArtistRepository artistRepository, IMappingEngine mapper, IODataQueryMapper odataMapper)
        {
            this.artistRepository = artistRepository;
            this.mapper = mapper;
            this.odataMapper = odataMapper;
        }

        public async Task<IEnumerable<DomainArtist>> GetArtists()
        {
            var artists = await this.artistRepository.GetArtists();

            return this.mapper.Map<IEnumerable<DomainArtist>>(artists);
        }

        public async Task<IEnumerable<DomainArtist>> GetArtists(ODataQueryOptions<DomainArtist> query)
        {
            var q = this.odataMapper.Map<DomainArtist, Artist>(query);

            var albums = await this.artistRepository.GetArtists(q);

            return this.mapper.Map<IEnumerable<DomainArtist>>(albums);
        }
    }
}