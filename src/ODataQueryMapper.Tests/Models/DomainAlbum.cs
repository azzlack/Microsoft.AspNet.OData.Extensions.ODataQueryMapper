namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models
{
    using System.Web.OData.Query;

    [Count]
    [Filter]
    [OrderBy]
    public class DomainAlbum
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public Resource<long> Artist { get; set; }
    }
}