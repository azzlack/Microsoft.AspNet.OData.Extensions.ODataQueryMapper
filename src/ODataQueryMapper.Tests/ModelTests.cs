namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Extensions;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Profiles;
    using NUnit.Framework;
    using System;
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
        public void CreateODataQuery_WhenFilterIsModified_ReturnsQuery()
        {
            var q = ODataQuery.Create<Album>("$filter=Id le 10&$orderby=Id&$top=10&$skip=5");
            var m = q.FilterExpression.And("contains(tolower(Title), 'rock')").Or("contains(tolower(Title), 'hell')").GetQuery();

            Assert.AreEqual(q.Options.Top.RawValue, m.Options.Top.RawValue);
            Assert.AreEqual(q.Options.Skip.RawValue, m.Options.Skip.RawValue);
            Assert.AreEqual(q.Options.OrderBy.RawValue, m.Options.OrderBy.RawValue);
            Assert.AreEqual("Id le 10 and contains(tolower(Title), 'rock') or contains(tolower(Title), 'hell')", m.Options.Filter.RawValue);
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

        [Test]
        public void IsEmpty_WhenGivenEmptyQuery_ReturnsTrue()
        {
            var q = ODataQuery.Create<Album>("").Options.IsEmpty();

            Assert.IsTrue(q);
        }

        [TestCase("$top=1")]
        [TestCase("$skip=1")]
        [TestCase("$count=true")]
        [TestCase("$filter=test")]
        [TestCase("$orderby=true")]
        public void IsEmpty_WhenGivenNonEmptyQuery_ReturnsFalse(string query)
        {
            var q = ODataQuery.Create<Album>(query).Options.IsEmpty();

            Assert.IsFalse(q);
        }

        [TestCase("$top=1")]
        [TestCase("$skip=10&$top=10")]
        [TestCase("$count=true")]
        [TestCase("$filter=test eq 'old'")]
        public void ToODataUriString_WhenGivenODataQuery_ReturnsQuery(string query)
        {
            var q = ODataQuery.Create<Album>(query).Options.ToODataUriString();

            Console.WriteLine("Query1: " + query);
            Console.WriteLine("Query2: " + q);

            Assert.AreEqual(query, q);
        }
    }
}