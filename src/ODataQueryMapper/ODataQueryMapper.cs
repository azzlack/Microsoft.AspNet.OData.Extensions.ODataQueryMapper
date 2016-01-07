namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.OData.Edm;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.OData;
    using System.Web.OData.Query;

    /// <summary>An OData query mapper.</summary>
    public class ODataQueryMapper : IODataQueryMapper
    {
        /// <summary>The singleton.</summary>
        private static readonly Lazy<IODataQueryMapper> Singleton = new Lazy<IODataQueryMapper>(() => new ODataQueryMapper(new ODataQueryOptionsFactory()));

        /// <summary>The odata query options factory.</summary>
        private readonly IODataQueryOptionsFactory odataDataQueryOptionsFactory;

        /// <summary>The configuration.</summary>
        private IConfiguration configuration;

        /// <summary>Initializes a new instance of the <see cref="ODataQueryMapper" /> class.</summary>
        /// <param name="odataDataQueryOptionsFactory">The odata query options factory.</param>
        internal ODataQueryMapper(IODataQueryOptionsFactory odataDataQueryOptionsFactory)
        {
            this.odataDataQueryOptionsFactory = odataDataQueryOptionsFactory;
        }

        /// <summary>Gets the configuration.</summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration
        {
            get
            {
                if (this.configuration == null)
                {
                    throw new InvalidOperationException("The Engine has not been initialized. Please run Initialize() before trying to retrieve the configuration.");
                }

                return this.configuration;
            }

            set
            {
                this.configuration = value;
            }
        }

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
            ((ODataQueryMapper)Engine).Setup(action);
            ((Configuration)Engine.Configuration).Verify();
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
            var map = ((Configuration)this.Configuration).GetMap<TSource>();

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

            var model = ((Configuration)this.Configuration).GetModel<TDestination>();

            var clauses = new Dictionary<string, string>();

            if (orderBy != null)
            {
                clauses.Add("$orderby", orderBy.ToString());
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

            var context = new ODataQueryContext(model, typeof(TDestination), query.Context.Path);

            return this.odataDataQueryOptionsFactory.Create<TDestination>(clauses, context, query.Request);
        }

        /// <summary>Creates a new version of the data model, containing the specified entity.</summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>The data model.</returns>
        public IEdmModel CreateModel<T>() where T : class
        {
            return ((Configuration)this.Configuration).GetModel<T>();
        }

        /// <summary>Gets the model.</summary>
        /// <returns>The model.</returns>
        public IEdmModel GetModel()
        {
            return this.Configuration.Model;
        }

        /// <summary>Initializes this object.</summary>
        /// <param name="initializeExpression">The initialization expression.</param>
        internal void Setup(Action<IConfiguration> initializeExpression)
        {
            this.Configuration = new Configuration(initializeExpression);
        }

        /// <summary>Replaces the source content according to the replacement map.</summary>
        /// <param name="source">The source.</param>
        /// <param name="replacementMap">The replacement map.</param>
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