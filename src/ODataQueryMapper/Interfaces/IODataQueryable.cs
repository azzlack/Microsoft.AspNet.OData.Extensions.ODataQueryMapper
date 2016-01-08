namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Serializers;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Threading.Tasks;

    [JsonConverter(typeof(ODataQueryableSerializer))]
    public interface IODataQueryable<T> : IQueryable<T>, IODataCollection<T>
    {
        /// <summary>Executes the current query.</summary>
        /// <returns>The committed query result.</returns>
        Task<IODataQueryable<T>> ToListAsync();
    }
}