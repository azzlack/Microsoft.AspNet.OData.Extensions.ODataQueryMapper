namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using System;
    using System.Collections.Generic;
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

        /// <summary>Gets the <see cref="ODataQueryOptions{T}"/> instance.</summary>
        /// <value>The ODataQueryOptions instance.</value>
        public ODataQueryOptions<T> Options => this;

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
                        var link = this.Request.GetNextPageLink(querySettings.PageSize.Value);

                        limitedResults.NextLink = link.ToString();
                        this.Request.ODataProperties().NextLink = link;
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
            return Create<T>(query, ODataQueryMapper.Engine);
        }

        /// <summary>Creates a new OData query from the specified query string.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <typeparam name="T">The OData type.</typeparam>
        /// <param name="query">The query string.</param>
        /// <param name="mapper">The mapper.</param>
        /// <returns>An OData query.</returns>
        public static IODataQuery<T> Create<T>(string query, IODataQueryMapper mapper) where T : class
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper), "You must specify an instance of IODataQueryMapper");
            }

            var modelBuilder = new ODataConventionModelBuilder();
            mapper.Configuration.GetTypeConfiguration<T>(modelBuilder);

            var model = modelBuilder.GetEdmModel();

            var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost/{typeof(T).Name}?{query}");
            var context = new ODataQueryContext(model, typeof(T), new ODataPath());

            return new ODataQuery<T>(context, request);
        }

        /// <summary>Creates a new OData query from the specified parameters.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="top">The number of items to take.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="orderBy">The properties to order by.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>An OData query.</returns>
        public static IODataQuery<T> Create<T>(int top = 0, int skip = 0, string orderBy = "", string filter = "") where T : class
        {
            return Create<T>(ODataQueryMapper.Engine, top, skip, orderBy, filter);
        }

        /// <summary>Creates a new OData query from the specified parameters.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="mapper">The mapper.</param>
        /// <param name="top">The number of items to take.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="orderBy">The properties to order by.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>An OData query.</returns>
        public static IODataQuery<T> Create<T>(IODataQueryMapper mapper, int top = 0, int skip = 0, string orderBy = "", string filter = "") where T : class
        {
            var query = new List<string>();

            if (top > 0)
            {
                query.Add($"$top={top}");
            }

            if (skip > 0)
            {
                query.Add($"$skip={skip}");
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                query.Add($"$orderby={orderBy}");
            }

            if (!string.IsNullOrEmpty(filter))
            {
                query.Add($"filter={filter}");
            }

            return Create<T>(string.Join("&", query), mapper);
        }

        /// <summary>Gets an empty OData query.</summary>
        /// <typeparam name="T">The OData type.</typeparam>
        /// <returns>An OData query.</returns>
        public static IODataQuery<T> Empty<T>() where T : class
        {
            return Empty<T>(ODataQueryMapper.Engine);
        }

        /// <summary>Gets an empty OData query.</summary>
        /// <typeparam name="T">The OData type.</typeparam>
        /// <param name="mapper">The mapper.</param>
        /// <returns>An OData query.</returns>
        public static IODataQuery<T> Empty<T>(IODataQueryMapper mapper) where T : class
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper), "You must specify an instance of IODataQueryMapper");
            }

            var modelBuilder = new ODataConventionModelBuilder();
            mapper.Configuration.GetTypeConfiguration<T>(modelBuilder);

            var model = modelBuilder.GetEdmModel();

            var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost/{typeof(T).Name}");
            var context = new ODataQueryContext(model, typeof(T), new ODataPath());

            return new ODataQuery<T>(context, request);
        }

        /// <summary>Gets a default OData query.</summary>
        /// <typeparam name="T">The OData type.</typeparam>
        /// <returns>An OData query.</returns>
        public static IODataQuery<T> Default<T>(HttpRequestMessage request) where T : class
        {
            return Default<T>(request, ODataQueryMapper.Engine);
        }

        /// <summary>Gets a default OData query.</summary>
        /// <typeparam name="T">The OData type.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="mapper">The mapper.</param>
        /// <returns>An OData query.</returns>
        public static IODataQuery<T> Default<T>(HttpRequestMessage request, IODataQueryMapper mapper) where T : class
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper), "You must specify an instance of IODataQueryMapper");
            }

            var modelBuilder = new ODataConventionModelBuilder();
            mapper.Configuration.GetTypeConfiguration<T>(modelBuilder);

            var model = modelBuilder.GetEdmModel();

            var context = new ODataQueryContext(model, typeof(T), new ODataPath());

            return new ODataQuery<T>(context, request);
        }
    }
}