namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models
{
    public class DomainTrack
    {
        public int Wbs { get; set; }

        public string Title { get; set; }

        public Resource<long> Album { get; set; }
    }
}