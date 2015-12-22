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

        /// <summary>Gets the data model.</summary>
        /// <returns>The data model.</returns>
        IEdmModel GetModel();
    }
}