namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using Microsoft.OData.Edm;
    using System.Web.OData.Query;

    /// <summary>Interface for an OData query mapper.</summary>
    public interface IODataQueryMapper
    {
        /// <summary>Gets the configuration.</summary>
        /// <value>The configuration.</value>
        IConfiguration Configuration { get; }

        /// <summary>Maps the specified query to the other type.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TDestination">The destination type.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>An OData query.</returns>
        IODataQuery<TDestination> Map<TSource, TDestination>(ODataQueryOptions<TSource> query)
            where TSource : class
            where TDestination : class;

        /// <summary>Creates a new version of the data model, containing the specified entity.</summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>The data model.</returns>
        IEdmModel CreateModel<T>() where T : class;

        /// <summary>Gets the model.</summary>
        /// <returns>The model.</returns>
        IEdmModel GetModel();
    }
}