namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class ODataCollection<T>
    {
        [JsonProperty("value")]
        public IEnumerable<T> Values { get; set; }

        [JsonProperty("@odata.count")]
        public int Count { get; set; }

        [JsonProperty("@odata.context")]
        public string Context { get; set; }
    }
}