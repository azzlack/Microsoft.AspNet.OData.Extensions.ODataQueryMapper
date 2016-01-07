namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models
{
    using System.Collections.Generic;

    public class DomainArtist
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public IEnumerable<DomainAlbum> Albums { get; set; }
    }
}