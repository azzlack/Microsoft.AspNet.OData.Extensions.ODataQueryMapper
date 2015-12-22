namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.OData.Core.UriParser;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.OData;
    using System.Web.OData.Query;

    public class ODataQuery<T> : IODataQuery<T>
    {
        private readonly FilterQueryOption filter;

        private readonly OrderByQueryOption orderBy;

        private TopQueryOption top;

        private SkipQueryOption skip;

        public ODataQuery(Dictionary<string, string> clauses, ODataQueryContext context, ODataQueryOptionParser parser)
        {
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
        }

        /// <summary>Applies to described by query.</summary>
        /// <param name="query">The query.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        public IQueryable<T> ApplyTo(IQueryable<T> query)
        {
            // TODO: Support count
            if (this.filter != null)
            {
                query = this.filter.ApplyTo(query, new ODataQuerySettings()) as IQueryable<T>;
            }

            if (this.orderBy != null)
            {
                query = this.orderBy.ApplyTo(query, new ODataQuerySettings());
            }

            if (this.skip != null)
            {
                query = this.skip.ApplyTo(query, new ODataQuerySettings());
            }

            if (this.top != null)
            {
                query = this.top.ApplyTo(query, new ODataQuerySettings());
            }

            return query;
        }
    }
}