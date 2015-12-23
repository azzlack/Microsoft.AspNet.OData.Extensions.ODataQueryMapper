namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using System.Collections.Generic;

    public interface ITypeConverter<TSource, TDestination>
    {
        /// <summary>Creates a mapping table.</summary>
        /// <returns>The mapping table.</returns>
        Dictionary<string, string> CreateMappingTable();
    }
}