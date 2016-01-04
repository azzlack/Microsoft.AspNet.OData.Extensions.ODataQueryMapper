namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using System.Collections.Generic;

    public interface IODataCollection<T>
    {
        /// <summary>Gets or sets the total number of items.</summary>
        /// <value>The total number of items.</value>
        int Count { get; set; }

        /// <summary>Gets or sets the link to the next item set.</summary>
        /// <value>The link to the next item set.</value>
        string NextLink { get; set; }

        /// <summary>Gets the data.</summary>
        /// <value>The data.</value>
        IEnumerable<T> Value { get; }
    }
}