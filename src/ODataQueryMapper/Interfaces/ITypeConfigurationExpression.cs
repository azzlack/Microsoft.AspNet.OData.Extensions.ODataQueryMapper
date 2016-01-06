namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using System;
    using System.Linq.Expressions;
    using System.Web.OData.Builder;

    public interface ITypeConfigurationExpression<T>
        where T : class
    {
        /// <summary>Configures the entity.</summary>
        /// <param name="entityConfiguration">The entity configuration expression.</param>
        /// <returns>This instance.</returns>
        ITypeConfigurationExpression<T> ForEntity(Expression<Action<EntityTypeConfiguration<T>>> entityConfiguration);
    }
}