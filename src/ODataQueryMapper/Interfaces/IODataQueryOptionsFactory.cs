namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.OData;

    public interface IODataQueryOptionsFactory
    {
        /// <summary>Creates a new ODataQueryOptions{T}</summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="clauses">The clauses.</param>
        /// <param name="context">The context.</param>
        /// <param name="request">The request.</param>
        /// <returns>An OData query object.</returns>
        IODataQuery<T> Create<T>(Dictionary<string, string> clauses, ODataQueryContext context, HttpRequestMessage request);

        /// <summary>Modifies the original query and returns a new one.</summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="clauses">The clauses.</param>
        /// <param name="original">The original query.</param>
        /// <returns>A new query.</returns>
        IODataQuery<T> Modify<T>(Dictionary<string, string> clauses, IODataQuery<T> original);
    }
}