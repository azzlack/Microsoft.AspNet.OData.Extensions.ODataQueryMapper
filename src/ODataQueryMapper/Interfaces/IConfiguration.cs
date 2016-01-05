namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using Microsoft.OData.Edm;
    using System.Collections.Generic;
    using System.Web.OData.Builder;

    public interface IConfiguration : IProfileConfiguration
    {
        /// <summary>Gets a value indicating whether the sealed.</summary>
        /// <value>true if sealed, false if not.</value>
        bool Sealed { get; }

        /// <summary>Gets the data model.</summary>
        /// <value>The data model.</value>
        IEdmModel Model { get; }

        /// <summary>Verifies this configuration.</summary>
        void Verify();

        /// <summary>Gets the transformation rules for the specified type.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <returns>The transformation rules.</returns>
        Dictionary<string, string> GetMap<TSource>();

        /// <summary>Gets the type configuration.</summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="modelBuilder">The model builder.</param>
        /// <returns>The type configuration.</returns>
        EntityTypeConfiguration<T> GetTypeConfiguration<T>(ODataConventionModelBuilder modelBuilder) where T : class;

        /// <summary>Adds the specified profile.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        void AddProfile<T>() where T : IMappingProfile;
    }
}