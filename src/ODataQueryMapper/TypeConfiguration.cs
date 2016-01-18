namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using System.Web.OData.Builder;

    public class TypeConfiguration<T> : ITypeConfiguration<T>
        where T : class
    {
        /// <summary>The entity type configuration.</summary>
        private readonly EntityTypeConfiguration<T> entityTypeConfiguration;

        /// <summary>The type configurations.</summary>
        private readonly Collection<LambdaExpression> typeConfigurations;

        /// <summary>The transform rules.</summary>
        private readonly Dictionary<string, string> rules;

        /// <summary>Initializes a new instance of the <see cref="TypeConfiguration{T}" /> class.</summary>
        /// <param name="entityTypeConfiguration">The entity type configuration.</param>
        /// <param name="typeConfigurations">The type configurations.</param>
        /// <param name="rules">The transform rules.</param>
        public TypeConfiguration(EntityTypeConfiguration<T> entityTypeConfiguration, Collection<LambdaExpression> typeConfigurations, Dictionary<string, string> rules)
        {
            this.entityTypeConfiguration = entityTypeConfiguration;
            this.typeConfigurations = typeConfigurations;
            this.rules = rules;
        }

        /// <summary>Creates a map between the two types.</summary>
        /// <typeparam name="TDestination">The destination type.</typeparam>
        /// <returns>The mapping expression.</returns>
        public IMappingExpression<T, TDestination> CreateMap<TDestination>() where TDestination : class
        {
            return new MappingExpression<T, TDestination>(this.typeConfigurations, this.rules);
        }

        /// <summary>Use a custom type configurator instance to configure the specified type.</summary>
        /// <typeparam name="TTypeConfigurator">The configurator type.</typeparam>
        public void ConfigureUsing<TTypeConfigurator>() where TTypeConfigurator : ITypeConfigurator<T>
        {
            var configurator = Activator.CreateInstance<TTypeConfigurator>();

            configurator.Setup(this.entityTypeConfiguration);
        }

        /// <summary>Configures the entity.</summary>
        /// <param name="entityConfiguration">The entity configuration expression.</param>
        /// <returns>This instance.</returns>
        public ITypeConfiguration<T> ForEntity(Expression<Action<EntityTypeConfiguration<T>>> entityConfiguration)
        {
            this.typeConfigurations.Add(entityConfiguration);

            return this;
        }
    }
}