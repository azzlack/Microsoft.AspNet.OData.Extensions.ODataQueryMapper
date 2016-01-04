namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;

    using Newtonsoft.Json;

    public class ODataCollection<T> : IODataCollection<T>
    {
        [DataMember(Name = "value")]
        [JsonProperty("value")]
        public IEnumerable<T> Value { get; set; }

        [DataMember(Name = "@odata.count")]
        [JsonProperty("@odata.count")]
        public int Count { get; set; }

        [DataMember(Name = "@odata.context")]
        [JsonProperty("@odata.context")]
        public string Context { get; set; }

        [DataMember(Name = "@odata.nextLink")]
        [JsonProperty("@odata.nextLink")]
        public string NextLink { get; set; }
    }
}