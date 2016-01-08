namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Newtonsoft.Json.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    public class ODataCollection<T> : IODataCollection<T>
    {
        [DataMember(Name = "value")]
        public IEnumerable<T> Value { get; set; }

        [DataMember(Name = "@odata.count")]
        public int Count { get; set; }

        [DataMember(Name = "@odata.context")]
        public string Context { get; set; }

        [DataMember(Name = "@odata.nextLink")]
        public string NextLink { get; set; }

        /// <summary>Initializes this object.</summary>
        /// <param name="collection">The collection.</param>
        /// <param name="count">The total number of items.</param>
        /// <param name="nextLink">The link to the next item set.</param>
        public void Initialize(IEnumerable collection, int count, string nextLink = null)
        {
            this.Count = count;
            this.NextLink = nextLink;

            if (collection is JArray)
            {
                this.Value = ((JArray)collection).Select(x => x.ToObject<T>());
            }
            else if (collection != null)
            {
                this.Value = collection.Cast<T>().ToList();
            }
            else
            {
                this.Value = Enumerable.Empty<T>();
            }
        }

        /// <summary>Gets the data.</summary>
        /// <returns>The data.</returns>
        public IEnumerable GetValue()
        {
            return this.Value;
        }
    }
}