namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using System.Linq;
    using System.Threading.Tasks;

    public interface IODataQueryable<T> : IQueryable<T>, IODataCollection<T>
    {
        /// <summary>Executes the current query.</summary>
        /// <returns>The committed query result.</returns>
        Task<IODataQueryable<T>> ToListAsync();
    }
}