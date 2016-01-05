namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Converters
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using System.Collections.Generic;

    public class TrackConverter : ITypeConverter<DomainTrack, Track>
    {
        /// <summary>Creates a mapping table.</summary>
        /// <returns>The mapping table.</returns>
        public Dictionary<string, string> CreateMappingTable()
        {
            return new Dictionary<string, string>()
                       {
                           { "Wbs", "TrackId" },
                           { "Title", "Name" },
                           { "Album/Value", "AlbumId" }
                       };
        }
    }
}