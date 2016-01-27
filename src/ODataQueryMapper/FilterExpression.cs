namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using System.Collections.Generic;

    public class FilterExpression<T> : IFilterExpression<T>
    {
        /// <summary>The query options factory.</summary>
        private readonly IODataQueryFactory queryFactory;

        /// <summary>The query.</summary>
        private IODataQuery<T> query;

        /// <summary>Initializes a new instance of the <see cref="FilterExpression{T}" /> class.</summary>
        /// <param name="queryFactory">The query options factory.</param>
        /// <param name="query">The query.</param>
        internal FilterExpression(IODataQueryFactory queryFactory, IODataQuery<T> query)
        {
            this.queryFactory = queryFactory;
            this.query = query;
        }

        /// <summary>Ands the current filter.</summary>
        /// <param name="filter">The new filter.</param>
        /// <returns>A filter expression.</returns>
        public IFilterExpression<T> And(string filter)
        {
            this.query = this.queryFactory.Modify(new Dictionary<string, string> { { "$filter", $" and {filter}" } }, this.query);

            return this;
        }

        /// <summary>Ors the current filter.</summary>
        /// <param name="filter">The new filter.</param>
        /// <returns>A filter expression.</returns>
        public IFilterExpression<T> Or(string filter)
        {
            this.query = this.queryFactory.Modify(new Dictionary<string, string> { { "$filter", $" or {filter}" } }, this.query);

            return this;
        }

        /// <summary>Gets the current query.</summary>
        /// <returns>An OData query</returns>
        public IODataQuery<T> GetQuery()
        {
            return this.query;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return this.query.Options.Filter.RawValue;
        }
    }
}