namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models
{
    public class DomainAlbum
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public Resource<long> Artist { get; set; }
    }
}