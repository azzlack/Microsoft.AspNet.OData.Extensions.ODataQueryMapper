namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    public interface IFilterExpression<T>
    {
        /// <summary>Ands the current filter.</summary>
        /// <param name="filter">The new filter.</param>
        /// <returns>A filter expression.</returns>
        IFilterExpression<T> And(string filter);

        /// <summary>Ors the current filter.</summary>
        /// <param name="filter">The new filter.</param>
        /// <returns>A filter expression.</returns>
        IFilterExpression<T> Or(string filter);

        /// <summary>Gets the current query.</summary>
        /// <returns>An OData query</returns>
        IODataQuery<T> GetQuery();
    }
}