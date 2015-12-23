namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.OData.Core.UriParser;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web.OData;
    using System.Web.OData.Extensions;
    using System.Web.OData.Query;

    public class ODataQuery<T> : IODataQuery<T>
    {
        private readonly HttpRequestMessage request;

        private readonly FilterQueryOption filter;

        private readonly OrderByQueryOption orderBy;

        private readonly TopQueryOption top;

        private readonly SkipQueryOption skip;

        private readonly CountQueryOption count;

        private readonly SelectExpandQueryOption selectExpand;

        public ODataQuery(Dictionary<string, string> clauses, ODataQueryContext context, HttpRequestMessage request, ODataQueryOptionParser parser)
        {
            this.request = request;

            if (clauses.ContainsKey("$filter"))
            {
                this.filter = new FilterQueryOption(clauses["$filter"], context, parser);
            }

            if (clauses.ContainsKey("$orderBy"))
            {
                this.orderBy = new OrderByQueryOption(clauses["$orderBy"], context, parser);
            }

            if (clauses.ContainsKey("$top"))
            {
                this.top = new TopQueryOption(clauses["$top"], context, parser);
            }

            if (clauses.ContainsKey("$skip"))
            {
                this.skip = new SkipQueryOption(clauses["$skip"], context, parser);
            }

            if (clauses.ContainsKey("$count"))
            {
                this.count = new CountQueryOption(clauses["$count"], context, parser);
            }

            if (clauses.ContainsKey("$select") || clauses.ContainsKey("$expand"))
            {
                this.selectExpand = new SelectExpandQueryOption(clauses["$select"], clauses["$expand"], context, parser);
            }
        }

        /// <summary>Applies the OData query to the specified collection.</summary>
        /// <param name="collection">The collection.</param>
        /// <returns>The processed collection.</returns>
        public IQueryable<T> ApplyTo(IQueryable<T> collection)
        {
            return this.ApplyTo(collection, new ODataQuerySettings());
        }

        /// <summary>Applies to described by query.</summary>
        /// <param name="collection">The collection.</param>
        /// <param name="querySettings">The query settings.</param>
        /// <returns>The processed collection.</returns>
        public IQueryable<T> ApplyTo(IQueryable<T> collection, ODataQuerySettings querySettings)
        {
            var filterCollection = this.filter?.ApplyTo(collection, new ODataQuerySettings()) as IQueryable<T>;
            if (filterCollection != null)
            {
                collection = filterCollection;
            }

            var entityCount = this.count?.GetEntityCount(collection);
            if (entityCount != null && this.request != null && !this.request.ODataProperties().TotalCount.HasValue)
            {
                this.request.ODataProperties().TotalCount = entityCount;
            }

            if (this.orderBy != null)
            {
                collection = this.orderBy.ApplyTo(collection, querySettings);
            }

            if (this.skip != null)
            {
                collection = this.skip.ApplyTo(collection, querySettings);
            }

            if (this.top != null)
            {
                collection = this.top.ApplyTo(collection, querySettings);
            }

            var selectExpandCollection = this.selectExpand?.ApplyTo(collection, querySettings) as IQueryable<T>;
            if (selectExpandCollection != null)
            {
                collection = selectExpandCollection;
            }

            if (querySettings.PageSize.HasValue)
            {
                bool resultsLimited;
                collection = ODataQueryOptions.LimitResults(collection, querySettings.PageSize.Value, out resultsLimited);

                if (resultsLimited && this.request?.ODataProperties().NextLink != null)
                {
                    this.request.ODataProperties().NextLink = this.request.GetNextPageLink(querySettings.PageSize.Value);
                }
            }

            return collection;
        }
    }
}