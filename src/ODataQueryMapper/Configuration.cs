namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.OData.Edm;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.OData.Builder;

    public class Configuration : IConfiguration
    {
        /// <summary>The edm builder.</summary>
        private readonly ODataModelBuilder builder;

        /// <summary>The type configurations.</summary>
        private readonly Dictionary<string, Collection<LambdaExpression>> typeConfigurations;

        /// <summary>The rules.</summary>
        private readonly Dictionary<string, Dictionary<string, string>> ruleSets;

        /// <summary>Initializes a new instance of the <see cref="Configuration" /> class.</summary>
        internal Configuration()
        {
            this.builder = new ODataConventionModelBuilder();

            this.typeConfigurations = new Dictionary<string, Collection<LambdaExpression>>();
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

        /// <summary>Gets the type configuration.</summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="modelBuilder">The model builder.</param>
        /// <returns>The type configuration.</returns>
        public EntityTypeConfiguration<T> GetTypeConfiguration<T>(ODataConventionModelBuilder modelBuilder) where T : class
        {
            modelBuilder.EntitySet<T>(typeof(T).Name);

            var config = modelBuilder.EntityType<T>();

            if (this.typeConfigurations.ContainsKey(typeof(T).FullName))
            {
                var configurationExpressions = this.typeConfigurations[typeof(T).FullName];

                foreach (var configurationExpression in configurationExpressions)
                {
                    var exp = configurationExpression.Compile() as Action<EntityTypeConfiguration<T>>;

                    if (exp != null)
                    {
                        exp(config);
                    }
                }
            }

            return config;
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

            var configurations = this.GetOrInsertTypeConfiguration(typeof(TDestination).FullName);
            var rules = this.GetOrInsertRuleSet(typeof(TSource).FullName);

            return new MappingExpression<TSource, TDestination>(configurations, rules);
        }

        /// <summary>Creates an entity set with the specified type and name.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="entitySetName">The entity name set.</param>
        /// <param name="configurationExpression">The configuration expression.</param>
        /// <returns>The type configuration.</returns>
        public ITypeConfiguration<TSource> Configure<TSource>(string entitySetName, Action<EntityTypeConfiguration<TSource>> configurationExpression) where TSource : class
        {
            this.builder.EntitySet<TSource>(entitySetName);

            var configurations = this.GetOrInsertTypeConfiguration(typeof(TSource).FullName);
            var rules = this.GetOrInsertRuleSet(typeof(TSource).FullName);

            configurationExpression(this.builder.EntityType<TSource>());

            return new TypeConfiguration<TSource>(this.builder.EntityType<TSource>(), configurations, rules);
        }

        /// <summary>Creates an entity set with the specified type and name.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="entitySetName">The entity name set.</param>
        /// <returns>The type configuration.</returns>
        public ITypeConfiguration<TSource> Configure<TSource>(string entitySetName) where TSource : class
        {
            this.builder.EntitySet<TSource>(entitySetName);

            var configurations = this.GetOrInsertTypeConfiguration(typeof(TSource).FullName);
            var rules = this.GetOrInsertRuleSet(typeof(TSource).FullName);

            return new TypeConfiguration<TSource>(this.builder.EntityType<TSource>(), configurations, rules);
        }

        /// <summary>Adds the specified profile.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        public void AddProfile<T>() where T : IMappingProfile
        {
            var profile = Activator.CreateInstance<T>();

            profile.Configure(this);
        }

        /// <summary>Gets or inserts a type configuration.</summary>
        /// <param name="typeConfigurationName">The type configuration name.</param>
        /// <returns>The ruleset.</returns>
        private Collection<LambdaExpression> GetOrInsertTypeConfiguration(string typeConfigurationName)
        {
            if (this.typeConfigurations.ContainsKey(typeConfigurationName))
            {
                return this.typeConfigurations[typeConfigurationName];
            }

            var configurations = new Collection<LambdaExpression>();
            this.typeConfigurations.Add(typeConfigurationName, configurations);

            return configurations;
        }

        /// <summary>Gets or inserts a ruleset.</summary>
        /// <param name="rulesetName">The ruleset name.</param>
        /// <returns>The ruleset.</returns>
        private Dictionary<string, string> GetOrInsertRuleSet(string rulesetName)
        {
            if (this.ruleSets.ContainsKey(rulesetName))
            {
                return this.ruleSets[rulesetName];
            }

            var rules = new Dictionary<string, string>();
            this.ruleSets.Add(rulesetName, rules);

            return rules;
        }
    }
}