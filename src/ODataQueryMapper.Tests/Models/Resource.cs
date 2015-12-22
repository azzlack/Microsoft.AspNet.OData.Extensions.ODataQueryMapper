namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models
{
    using Newtonsoft.Json;

    public class Resource<T>
    {
        [JsonProperty("value")]
        public T Value { get; set; }

        [JsonProperty("href")]
        public string Link { get; set; }

        [JsonProperty("rel")]
        public string Relation { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}