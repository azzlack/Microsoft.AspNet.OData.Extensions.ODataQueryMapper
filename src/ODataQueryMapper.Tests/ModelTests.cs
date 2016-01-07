namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Profiles;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web.OData.Query;

    [TestFixture]
    public class ModelTests
    {
        [SetUp]
        public void SetUp()
        {
            ODataQueryMapper.Initialize(
                x =>
                {
                    x.CreateMap<DomainAlbum, Album>("album")
                        .ForDestinationEntity(y => y.HasKey(z => z.AlbumId))
                        .ForMember(y => y.Id, y => y.AlbumId);

                    x.AddProfile<TestProfile>();
                });
        }

        [Test]
        public void CreateODataQuery_WhenGivenValidQueryString_ReturnsQuery()
        {
            var q = ODataQuery.Create<Album>("$filter=Id eq 1");

            Assert.IsNotNull(q);
        }

        [Test]
        public void ApplyODataQuery_WhenGivenValidAlbumQueryString_ReturnsFilteredCollection()
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

        [Test]
        public void ApplyODataQuery_WhenGivenValidTrackQueryString_ReturnsFilteredCollection()
        {
            var c = new List<Track>()
                        {
                            new Track() { TrackId = 1 },
                            new Track() { TrackId = 2 }
                        }.AsQueryable();
            var q = ODataQuery.Create<Track>("$filter=TrackId eq 1");

            var p = q.ApplyTo(c);

            Assert.AreEqual(1, p.Count());
        }

        [Test]
        public void ApplyODataQuery_WhenGivenEmpty_ReturnsFilteredCollection()
        {
            var c = new List<Album>()
                        {
                            new Album() { AlbumId = 1 },
                            new Album() { AlbumId = 2 }
                        }.AsQueryable();
            var q = ODataQuery.Empty<Album>();

            var p = q.ApplyTo(c);

            Assert.AreEqual(2, p.Count);
            Assert.AreEqual(2, p.Count());
        }

        [Test]
        public void ApplyODataQuery_WhenGivenDefault_ReturnsFilteredCollection()
        {
            var c = new List<Album>()
                        {
                            new Album() { AlbumId = 1 },
                            new Album() { AlbumId = 2 }
                        }.AsQueryable();
            var q =
                ODataQuery.Default<Album>(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/album")
                    {
                        Properties =
                                {
                                    {
                                        "odata.QuerySettings",
                                        new ODataQuerySettings()
                                            {
                                                PageSize = 1
                                            }
                                    }
                                }
                    });

            var p = q.ApplyTo(c);

            Assert.AreEqual(2, p.Count);
            Assert.IsNotNull(p.NextLink);
            Assert.AreEqual(1, p.Count());
        }
    }
}