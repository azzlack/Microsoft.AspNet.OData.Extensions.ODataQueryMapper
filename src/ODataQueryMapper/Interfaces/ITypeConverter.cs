namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using System.Collections.Generic;

    public interface ITypeConverter<TSource, TDestination>
        where TSource : class
        where TDestination : class
    {
        /// <summary>Creates a mapping table.</summary>
        /// <returns>The mapping table.</returns>
        Dictionary<string, string> CreateMappingTable();
    }
}