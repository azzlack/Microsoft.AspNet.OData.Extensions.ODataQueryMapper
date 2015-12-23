namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using System.Linq;
    using System.Web.OData.Query;

    public interface IODataQuery<T>
    {
        /// <summary>Applies the OData query to the specified collection.</summary>
        /// <param name="collection">The collection.</param>
        /// <returns>The processed collection.</returns>
        IODataQueryable<T> ApplyTo(IQueryable<T> collection);

        /// <summary>Applies to described by query.</summary>
        /// <param name="collection">The collection.</param>
        /// <param name="querySettings">The query settings.</param>
        /// <returns>The processed collection.</returns>
        IODataQueryable<T> ApplyTo(IQueryable<T> collection, ODataQuerySettings querySettings);
    }
}