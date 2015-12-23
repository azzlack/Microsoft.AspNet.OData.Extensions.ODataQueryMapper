namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IODataQueryable<T> : IQueryable<T>
    {
        /// <summary>Gets the total number of items.</summary>
        /// <value>The total number of items.</value>
        int Total { get; }

        /// <summary>Gets or sets the link to the next item set.</summary>
        /// <value>The link to the next item set.</value>
        Uri NextLink { get; set; }

        /// <summary>Converts this object to a list asynchronously.</summary>
        /// <returns>The list.</returns>
        Task<List<T>> ToListAsync();
    }
}