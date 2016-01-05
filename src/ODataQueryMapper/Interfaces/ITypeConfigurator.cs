using System.Web.OData.Builder;

namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    public interface ITypeConfigurator<T> where T : class
    {
        /// <summary>Configures the entity type.</summary>
        /// <param name="entityTypeConfiguration">The entity type configuration.</param>
        void Setup(EntityTypeConfiguration<T> entityTypeConfiguration);
    }
}