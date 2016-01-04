namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IODataQueryable<T> : IQueryable<T>, IODataCollection<T>
    {
        /// <summary>Converts this object to a list asynchronously.</summary>
        /// <returns>The list.</returns>
        Task<List<T>> ToListAsync();
    }
}