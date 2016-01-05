namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    public interface ITypeConfiguration<TSource> where TSource : class
    {
        /// <summary>Creates a map between the two types.</summary>
        /// <typeparam name="TDestination">The destination type.</typeparam>
        /// <returns>The mapping expression.</returns>
        IMappingExpression<TSource, TDestination> CreateMap<TDestination>()
            where TDestination : class;

        /// <summary>Use a custom type configurator instance to configure the specified type.</summary>
        /// <typeparam name="TTypeConfigurator">The configurator type.</typeparam>
        void ConfigureUsing<TTypeConfigurator>() where TTypeConfigurator : ITypeConfigurator<TSource>;
    }
}