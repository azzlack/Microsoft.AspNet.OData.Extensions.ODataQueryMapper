namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Profiles
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Configurators;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Converters;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;

    public class TestProfile : IMappingProfile
    {
        /// <summary>Method for configuring mappers.</summary>
        /// <param name="configuration">The configuration.</param>
        public void Configure(IProfileConfiguration configuration)
        {
            configuration.CreateMap<DomainArtist, Artist>("artist").ConvertUsing<ArtistConverter>();

            configuration.Configure<DomainTrack>("track").ConfigureUsing<TrackConfigurator>(); // TODO: ITypeConfigurator must be able to map stuff
            configuration.Configure<DomainTrack>("track").ForEntity(x => x.HasKey(y => y.Wbs)).CreateMap<Track>().ConvertUsing<TrackConverter>();

            configuration.Configure<Track>("Track", x => x.HasKey(y => y.TrackId));
        }
    }
}