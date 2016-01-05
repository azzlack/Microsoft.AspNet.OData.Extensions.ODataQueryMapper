namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Configurators
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using System.Web.OData.Builder;

    public class TrackConfigurator : ITypeConfigurator<DomainTrack>
    {
        /// <summary>Configures the entity type.</summary>
        /// <param name="entityTypeConfiguration">The entity type configuration.</param>
        public void Setup(EntityTypeConfiguration<DomainTrack> entityTypeConfiguration)
        {
            entityTypeConfiguration.HasKey(x => x.Wbs);
        }
    }
}