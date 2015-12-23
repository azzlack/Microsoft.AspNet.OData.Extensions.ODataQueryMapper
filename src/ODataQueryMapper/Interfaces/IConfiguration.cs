namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using Microsoft.OData.Edm;
    using System.Collections.Generic;

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
        /// <typeparam name="TSource">Type of the source.</typeparam>
        /// <returns>The transformation rules.</returns>
        Dictionary<string, string> GetMap<TSource>();

        /// <summary>Adds the specified profile.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        void AddProfile<T>() where T : IMappingProfile;
    }
}