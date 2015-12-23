namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Edm;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.OData;
    using System.Web.OData.Builder;
    using System.Web.OData.Query;

    /// <summary>An OData query mapper.</summary>
    public class ODataQueryMapper : IODataQueryMapper
    {
        /// <summary>The singleton.</summary>
        private static readonly Lazy<IODataQueryMapper> Singleton = new Lazy<IODataQueryMapper>(() => new ODataQueryMapper());

        /// <summary>Initializes a new instance of the <see cref="ODataQueryMapper" /> class.</summary>
        internal ODataQueryMapper()
        {
        }

        /// <summary>Gets the configuration.</summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; private set; }

        /// <summary>The engine.</summary>
        public static IODataQueryMapper Engine => Singleton.Value;

        /// <summary>
        /// Initializes a mapper with the supplied configuration. Runtime optimization complete after this method is called. This is the preferred means to
        /// configure ODataQueryMapper.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>An OData query mapper.</returns>
        public static void Initialize(Action<IConfiguration> action)
        {
            ((ODataQueryMapper)Engine).Reset();
            action(Engine.Configuration);
            Engine.Configuration.Verify();
        }

        /// <summary>Maps the specified query to the other type.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TDestination">The destination type.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>An OData query.</returns>
        public IODataQuery<TDestination> Map<TSource, TDestination>(ODataQueryOptions<TSource> query)
            where TSource : class
            where TDestination : class
        {
            // 1. Get map
            var map = this.Configuration.GetMap<TSource>();

            // 2. Transform clauses
            StringBuilder orderBy = null;
            if (query.OrderBy != null)
            {
                orderBy = new StringBuilder(query.OrderBy.RawValue);
                this.BulkReplace(orderBy, map);
            }

            StringBuilder filter = null;
            if (query.Filter != null)
            {
                filter = new StringBuilder(query.Filter.RawValue);
                this.BulkReplace(filter, map);
            }

            // 3. Create new ODataQueryOptions for TDestination
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<TDestination>(typeof(TDestination).Name);

            var model = modelBuilder.GetEdmModel();
            var type = model.FindDeclaredType(typeof(TDestination).FullName);
            var source = model.FindDeclaredEntitySet(typeof(TDestination).Name);

            var clauses = new Dictionary<string, string>();

            if (orderBy != null)
            {
                clauses.Add("$orderBy", orderBy.ToString());
            }

            if (filter != null)
            {
                clauses.Add("$filter", filter.ToString());
            }

            if (query.Top != null)
            {
                clauses.Add("$top", query.Top.RawValue);
            }

            if (query.Skip != null)
            {
                clauses.Add("$skip", query.Skip.RawValue);
            }

            if (query.Count != null)
            {
                clauses.Add("$count", query.Count.RawValue);
            }

            if (query.SelectExpand != null)
            {
                clauses.Add("$select", query.SelectExpand.RawSelect);
                clauses.Add("$expand", query.SelectExpand.RawExpand);
            }

            var parser = new ODataQueryOptionParser(model, type, source, clauses);
            var context = new ODataQueryContext(model, typeof(TDestination), query.Context.Path);

            return new ODataQuery<TDestination>(clauses, context, query.Request, parser);
        }

        /// <summary>Gets the data model.</summary>
        /// <returns>The data model.</returns>
        public IEdmModel GetModel()
        {
            if (!this.Configuration.Sealed)
            {
                throw new InvalidOperationException("The configuration must be verified before the data model can be retrieved.");
            }

            return this.Configuration.Model;
        }

        /// <summary>Resets this object.</summary>
        internal void Reset()
        {
            this.Configuration = new Configuration();
        }

        /// <summary>
        /// Replaces the source content according to the replacement map.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="replacementMap">The replacement map.</param>
        /// <returns>The transformed content.</returns>
        private void BulkReplace(StringBuilder source, IDictionary<string, string> replacementMap)
        {
            if (source.Length == 0 || replacementMap.Count == 0)
            {
                return;
            }

            foreach (var map in replacementMap)
            {
                source.Replace(map.Key, map.Value);
            }
        }
    }
}