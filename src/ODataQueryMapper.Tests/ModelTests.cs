namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class ModelTests
    {
        [Test]
        public void CreateODataQuery_WhenGivenValidQueryString_ReturnsQuery()
        {
            var q = ODataQuery.Create<Album>("$filter=Id eq 1");

            Assert.IsNotNull(q);
        }

        [Test]
        public void ApplyODataQuery_WhenGivenValidQueryString_ReturnsFilteredCollection()
        {
            var c = new List<Album>()
                        {
                            new Album() { AlbumId = 1 },
                            new Album() { AlbumId = 2 }
                        }.AsQueryable();
            var q = ODataQuery.Create<Album>("$filter=AlbumId eq 1");

            var p = q.ApplyTo(c);

            Assert.AreEqual(1, p.Count());
        }
    }
}