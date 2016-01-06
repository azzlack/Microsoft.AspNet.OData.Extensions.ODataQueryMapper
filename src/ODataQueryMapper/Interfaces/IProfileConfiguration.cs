namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using System;
    using System.Web.OData.Builder;

    public interface IProfileConfiguration
    {
        /// <summary>Creates a map between the two types.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TDestination">The destination type.</typeparam>
        /// <param name="entitySetName">The entity name set.</param>
        /// <returns>The mapping expression.</returns>
        IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>(string entitySetName)
            where TSource : class
            where TDestination : class;

        /// <summary>Creates an entity set with the specified type.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <returns>The type configuration.</returns>
        ITypeConfiguration<TSource> Configure<TSource>() where TSource : class;

        /// <summary>Creates an entity set with the specified type and name.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="entitySetName">The entity name set.</param>
        /// <returns>The type configuration.</returns>
        ITypeConfiguration<TSource> Configure<TSource>(string entitySetName) where TSource : class;

        /// <summary>Creates an entity set with the specified type and name.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="entitySetName">The entity name set.</param>
        /// <param name="configurationExpression">The configuration expression.</param>
        /// <returns>The type configuration.</returns>
        ITypeConfiguration<TSource> Configure<TSource>(string entitySetName, Action<EntityTypeConfiguration<TSource>> configurationExpression) where TSource : class;
    }
}