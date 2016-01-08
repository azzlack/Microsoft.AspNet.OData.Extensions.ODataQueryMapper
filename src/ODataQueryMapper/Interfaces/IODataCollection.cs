namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Serializers;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;

    public interface IODataCollection<T> : IODataCollection
    {
        /// <summary>Gets the data.</summary>
        /// <value>The data.</value>
        IEnumerable<T> Value { get; }
    }

    [JsonConverter(typeof(ODataCollectionSerializer))]
    public interface IODataCollection
    {
        /// <summary>Gets or sets the total number of items.</summary>
        /// <value>The total number of items.</value>
        int Count { get; set; }

        /// <summary>Gets or sets the link to the next item set.</summary>
        /// <value>The link to the next item set.</value>
        string NextLink { get; set; }

        /// <summary>Initializes this object.</summary>
        /// <param name="collection">The collection.</param>
        /// <param name="count">The total number of items.</param>
        /// <param name="nextLink">The link to the next item set.</param>
        void Initialize(IEnumerable collection, int count, string nextLink = null);

        /// <summary>Gets the data.</summary>
        /// <returns>The data.</returns>
        IEnumerable GetValue();
    }
}