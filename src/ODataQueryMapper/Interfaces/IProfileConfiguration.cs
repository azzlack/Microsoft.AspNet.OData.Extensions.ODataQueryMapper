namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    public interface IProfileConfiguration
    {
        /// <summary>Creates a map between the two types.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TDestination">The destination type.</typeparam>
        /// <param name="entitySetName">The entity name set.</param>
        /// <returns>The mapping expression.</returns>
        IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>(string entitySetName)
            where TSource : class
            where TDestination : class;
    }
}