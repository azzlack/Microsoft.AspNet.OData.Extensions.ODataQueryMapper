namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Functions
{
    using AutoMapper;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.OData.Query;

    public class TrackFunctions : ITrackFunctions
    {
        private readonly ITrackRepository trackRepository;

        private readonly IMappingEngine mapper;

        private readonly IODataQueryMapper odataMapper;

        public TrackFunctions(ITrackRepository trackRepository, IMappingEngine mapper, IODataQueryMapper odataMapper)
        {
            this.trackRepository = trackRepository;
            this.mapper = mapper;
            this.odataMapper = odataMapper;
        }

        public async Task<DomainTrack> GetTrack(int id)
        {
            var track = await this.trackRepository.GetTrack(id);

            return this.mapper.Map<DomainTrack>(track);
        }

        public async Task<IEnumerable<DomainTrack>> GetAlbumTracks(long albumId)
        {
            var tracks = await this.trackRepository.GetTracks(ODataQuery.Create<Track>($"$filter=AlbumId eq {albumId}"));

            return this.mapper.Map<IEnumerable<DomainTrack>>(tracks);
        }

        public async Task<IEnumerable<DomainTrack>> GetTracks(ODataQueryOptions<DomainTrack> query)
        {
            var q = this.odataMapper.Map<DomainTrack, Track>(query);

            var tracks = await this.trackRepository.GetTracks(q);

            return this.mapper.Map<IEnumerable<DomainTrack>>(tracks);
        }
    }
}