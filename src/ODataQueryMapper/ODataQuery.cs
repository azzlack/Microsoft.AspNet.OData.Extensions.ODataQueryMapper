namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using System.Linq;
    using System.Net.Http;
    using System.Web.OData;
    using System.Web.OData.Query;

    public class ODataQuery<T> : ODataQueryOptions<T>, IODataQuery<T>
    {
        /// <summary>Initializes a new instance of the <see cref="ODataQuery{T}"/> class.</summary>
        /// <param name="context">The context.</param>
        /// <param name="request">The request.</param>
        public ODataQuery(ODataQueryContext context, HttpRequestMessage request)
            : base(context, request)
        {
        }

        /// <summary>Applies the OData query to the specified collection.</summary>
        /// <param name="collection">The collection.</param>
        /// <returns>The processed collection.</returns>
        public IQueryable<T> ApplyTo(IQueryable<T> collection)
        {
            this.ValidateQuery();

            return base.ApplyTo(collection) as IQueryable<T>;
        }

        /// <summary>Applies to described by query.</summary>
        /// <param name="collection">The collection.</param>
        /// <param name="querySettings">The query settings.</param>
        /// <returns>The processed collection.</returns>
        public IQueryable<T> ApplyTo(IQueryable<T> collection, ODataQuerySettings querySettings)
        {
            this.ValidateQuery();

            return base.ApplyTo(collection, querySettings) as IQueryable<T>;
        }

        /// <summary>Validates the query.</summary>
        private void ValidateQuery()
        {
            var settings = this.Request.ODataValidationSettings();

            if (settings != null)
            {
                this.Validate(settings);
            }
        }
    }
}