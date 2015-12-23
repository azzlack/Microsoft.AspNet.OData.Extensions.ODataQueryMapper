namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.OData.Edm;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.OData.Builder;

    public class Configuration : IConfiguration
    {
        /// <summary>The edm builder.</summary>
        private readonly ODataModelBuilder builder;

        /// <summary>The rules.</summary>
        private readonly Dictionary<string, Dictionary<string, string>> ruleSets;

        /// <summary>Initializes a new instance of the <see cref="Configuration" /> class.</summary>
        internal Configuration()
        {
            this.builder = new ODataConventionModelBuilder();
            this.ruleSets = new Dictionary<string, Dictionary<string, string>>();
        }

        /// <summary>Gets a value indicating whether the sealed.</summary>
        /// <value>true if sealed, false if not.</value>
        public bool Sealed { get; private set; }

        /// <summary>Gets the data model.</summary>
        /// <value>The data model.</value>
        public IEdmModel Model { get; private set; }

        /// <summary>Verifies this configuration.</summary>
        public void Verify()
        {
            // TODO: Verify settings
            this.Sealed = true;

            this.Model = this.builder.GetEdmModel();
        }

        /// <summary>Gets the transformation rules for the specified type.</summary>
        /// <typeparam name="TSource">Type of the source.</typeparam>
        /// <returns>The transformation rules.</returns>
        public Dictionary<string, string> GetMap<TSource>()
        {
            if (this.ruleSets.ContainsKey(typeof(TSource).FullName))
            {
                return this.ruleSets[typeof(TSource).FullName].OrderByDescending(x => x.Key.Length).ToDictionary(x => x.Key, x => x.Value);
            }

            return new Dictionary<string, string>();
        }

        /// <summary>Creates a map between the two types.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TDestination">The destination type.</typeparam>
        /// <param name="entitySetName">The entity name set.</param>
        /// <returns>The new map.</returns>
        public IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>(string entitySetName)
            where TSource : class
            where TDestination : class
        {
            this.builder.EntitySet<TSource>(entitySetName);

            var rules = new Dictionary<string, string>();
            this.ruleSets.Add(typeof(TSource).FullName, rules);

            return new MappingExpression<TSource, TDestination>(rules);
        }
    }
}