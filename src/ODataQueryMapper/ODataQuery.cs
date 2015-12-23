namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{

    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using System.Linq;
    using System.Net.Http;
    using System.Web.OData;
    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using System.Web.OData.Query;
    using System.Web.OData.Routing;

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
        public IODataQueryable<T> ApplyTo(IQueryable<T> collection)
        {
            this.ValidateQuery();

            var querySettings = this.Request.ODataQuerySettings();

            if (querySettings != null)
            {
                var total = collection.Count();

                var settingsResult = base.ApplyTo(
                    collection,
                    new ODataQuerySettings()
                    {
                        EnsureStableOrdering = querySettings.EnsureStableOrdering,
                        EnableConstantParameterization = querySettings.EnableConstantParameterization,
                        HandleNullPropagation = querySettings.HandleNullPropagation
                    }) as IQueryable<T>;

                if (querySettings.PageSize.HasValue)
                {
                    int resultsRemoved;
                    var limitedResults = this.LimitResults(settingsResult, total, querySettings.PageSize.Value, out resultsRemoved);

                    if (resultsRemoved > 0 && this.Request.RequestUri != null && this.Request.ODataProperties().NextLink == null)
                    {
                        limitedResults.NextLink = this.Request.GetNextPageLink(querySettings.PageSize.Value);
                        this.Request.ODataProperties().NextLink = limitedResults.NextLink;
                    }

                    return limitedResults;
                }

                return new ODataQueryable<T>(settingsResult, settingsResult.Count());
            }

            var result = base.ApplyTo(collection) as IQueryable<T>;

            return new ODataQueryable<T>(result, result.Count());
        }

        /// <summary>Applies to described by query.</summary>
        /// <param name="collection">The collection.</param>
        /// <param name="querySettings">The query settings.</param>
        /// <returns>The processed collection.</returns>
        public IODataQueryable<T> ApplyTo(IQueryable<T> collection, ODataQuerySettings querySettings)
        {
            this.ValidateQuery();

            var result = base.ApplyTo(collection, querySettings) as IQueryable<T>;

            return new ODataQueryable<T>(result, result.Count());
        }

        private IODataQueryable<T> LimitResults(IQueryable<T> collection, int total, int limit, out int resultsRemoved)
        {
            resultsRemoved = total - limit;

            return new ODataQueryable<T>(collection.Take(limit), total);
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

    public static class ODataQuery
    {
        /// <summary>Creates a new OData query from the specified query string.</summary>
        /// <typeparam name="T">The OData type.</typeparam>
        /// <param name="query">The query string.</param>
        /// <returns>An OData query.</returns>
        public static IODataQuery<T> Create<T>(string query) where T : class
        {
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<T>(typeof(T).Name);

            var model = modelBuilder.GetEdmModel();

            var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost/{typeof(T).Name}?{query}");
            var context = new ODataQueryContext(model, typeof(T), new ODataPath());

            return new ODataQuery<T>(context, request);
        }
    }
}