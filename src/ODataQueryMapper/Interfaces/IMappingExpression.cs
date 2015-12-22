namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using System;
    using System.Linq.Expressions;

    public interface IMappingExpression<TSource, TDestination>
        where TSource : class
        where TDestination : class
    {
        /// <summary>Set configuration for individual properties.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="sourceMember">
        /// Expression to the top-level source member. This must be a member on the <typeparamref name="TSource"/>TSource.
        /// </param>
        /// <param name="destinationMember">
        /// Expression to the top-level destination member. This must be a member on the <typeparamref name="TDestination"/>TDestination.
        /// </param>
        /// <returns>This instance.</returns>
        IMappingExpression<TSource, TDestination> ForMember<T>(Expression<Func<TSource, T>> sourceMember, Expression<Func<TDestination, T>> destinationMember);

        /// <summary>Set configuration for individual properties.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="sourceProperty">
        /// Expression to the top-level source member. This must be a member on the <typeparamref name="TSource"/>TSource.
        /// </param>
        /// <param name="destinationMember">
        /// Expression to the top-level destination member. This must be a member on the <typeparamref name="TDestination"/>TDestination.
        /// </param>
        /// <returns>This instance.</returns>
        IMappingExpression<TSource, TDestination> ForMember<T>(string sourceProperty, Expression<Func<TDestination, T>> destinationMember);

        /// <summary>Set configuration for individual properties.</summary>
        /// <param name="sourceProperty">
        /// Expression to the top-level source member. This must be a member on the <typeparamref name="TSource"/>TSource.
        /// </param>
        /// <param name="destinationProperty">Destination property.</param>
        /// <returns>This instance.</returns>
        IMappingExpression<TSource, TDestination> ForMember(string sourceProperty, string destinationProperty);
    }
}