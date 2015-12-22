namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using System.Linq;

    public interface IODataQuery<T>
    {
        /// <summary>Applies to described by query.</summary>
        /// <param name="query">The query.</param>
        /// <returns>An IQueryable&lt;T&gt;</returns>
        IQueryable<T> ApplyTo(IQueryable<T> query);
    }
}