namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using Microsoft.OData.Edm;
    using System.Collections.Generic;

    public interface IConfiguration
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

        /// <summary>Creates a map between the two types.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TDestination">The destination type.</typeparam>
        /// <param name="entitySetName">The entity name set.</param>
        /// <returns>The mapping expression.</returns>
        IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>(string entitySetName)
            where TSource : class
            where TDestination : class;
    }
}