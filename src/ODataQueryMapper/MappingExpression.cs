namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using System.Web.OData.Builder;

    public class MappingExpression<TSource, TDestination> : IMappingExpression<TSource, TDestination>
        where TSource : class
        where TDestination : class
    {
        /// <summary>The type configurations.</summary>
        private readonly Collection<LambdaExpression> typeConfigurations;

        /// <summary>The transform rules.</summary>
        private readonly Dictionary<string, string> rules;

        /// <summary>Initializes a new instance of the <see cref="MappingExpression{TSource, TDestination}"/> class.</summary>
        /// <param name="typeConfigurations">The type configurations.</param>
        /// <param name="rules">The transform rules.</param>
        internal MappingExpression(Collection<LambdaExpression> typeConfigurations, Dictionary<string, string> rules)
        {
            this.typeConfigurations = typeConfigurations;
            this.rules = rules;
        }

        /// <summary>Set configuration for individual properties.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="sourceMember">
        /// Expression to the top-level source member. This must be a member on the <typeparamref name="TSource"/>.
        /// </param>
        /// <param name="destinationMember">
        /// Expression to the top-level destination member. This must be a member on the <typeparamref name="TDestination"/>.
        /// </param>
        /// <returns>This instance.</returns>
        public IMappingExpression<TSource, TDestination> ForMember<T>(Expression<Func<TSource, T>> sourceMember, Expression<Func<TDestination, T>> destinationMember)
        {
            var sourceExpression = this.ExtractExpression(sourceMember);
            var destinationExpression = this.ExtractExpression(destinationMember);

            return this.ForMember(sourceExpression.Member.Name, destinationExpression.Member.Name);
        }

        /// <summary>Set configuration for individual properties.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="sourceProperty">
        /// Expression to the top-level source member. This must be a member on the <typeparamref name="TSource"/>TSource.
        /// </param>
        /// <param name="destinationMember">
        /// Expression to the top-level destination member. This must be a member on the <typeparamref name="TDestination"/>TDestination.
        /// </param>
        /// <returns>This instance.</returns>
        public IMappingExpression<TSource, TDestination> ForMember<T>(string sourceProperty, Expression<Func<TDestination, T>> destinationMember)
        {
            var destinationExpression = this.ExtractExpression(destinationMember);

            return this.ForMember(sourceProperty, destinationExpression.Member.Name);
        }

        /// <summary>Set configuration for individual properties.</summary>
        /// <param name="sourceProperty">
        /// Expression to the top-level source member. This must be a member on the <typeparamref name="TSource"/>TSource.
        /// </param>
        /// <param name="destinationProperty">Destination property.</param>
        /// <returns>This instance.</returns>
        public IMappingExpression<TSource, TDestination> ForMember(string sourceProperty, string destinationProperty)
        {
            this.rules.Add(sourceProperty, destinationProperty);

            return this;
        }

        /// <summary>Configures the destination entity.</summary>
        /// <param name="entityConfiguration">The entity configuration expression.</param>
        /// <returns>This instance.</returns>
        public IMappingExpression<TSource, TDestination> ForDestinationEntity(Expression<Action<EntityTypeConfiguration<TDestination>>> entityConfiguration)
        {
            this.typeConfigurations.Add(entityConfiguration);

            return this;
        }

        /// <summary>Use a custom type converter instance to convert to the destination type.</summary>
        /// <typeparam name="TTypeConverter">The converter type.</typeparam>
        public void ConvertUsing<TTypeConverter>() where TTypeConverter : ITypeConverter<TSource, TDestination>
        {
            this.rules.Clear();
            foreach (var rule in Activator.CreateInstance<TTypeConverter>().CreateMappingTable())
            {
                this.rules.Add(rule.Key, rule.Value);
            }
        }

        private MemberExpression ExtractExpression<T1, T2>(Expression<Func<T1, T2>> expression)
        {
            var memberExpression = expression.Body as MemberExpression ?? ((UnaryExpression)expression.Body).Operand as MemberExpression;

            if (memberExpression == null)
            {
                throw new InvalidOperationException($"Unable to map property to {expression.Compile()}");
            }

            return memberExpression;
        }
    }
}