namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Functions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.OData.Query;

    using AutoMapper;

    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;

    public class ArtistFunctions : IArtistFunctions
    {
        private readonly IArtistRepository artistRepository;

        private readonly IMappingEngine mapper;

        public ArtistFunctions(IArtistRepository artistRepository, IMappingEngine mapper)
        {
            this.artistRepository = artistRepository;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<DomainArtist>> GetArtists()
        {
            var artists = await this.artistRepository.GetArtists();

            return this.mapper.Map<IEnumerable<DomainArtist>>(artists);
        }

        public Task<IEnumerable<DomainArtist>> GetArtists(ODataQueryOptions<DomainArtist> query)
        {
            throw new System.NotImplementedException();
        }
    }
}