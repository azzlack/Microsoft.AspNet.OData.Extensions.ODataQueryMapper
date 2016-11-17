﻿namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Extensions;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Profiles;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Hosting;
    using System.Web.OData.Query;

    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Enumerations;

    [TestFixture]
    public class ModelTests
    {
        private HttpConfiguration httpConfiguration;

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

            this.httpConfiguration = new HttpConfiguration();
            this.httpConfiguration.Properties.AddOrUpdate(ODataKeys.NonODataRootContainerKey, new ODataServiceProvider(ODataQueryMapper.Engine.Configuration), (oldValue, newValue) => newValue);
        }

        [Test]
        public void CreateODataQuery_WhenGivenValidQueryString_ReturnsQuery()
        {
            var q = ODataQuery.Create<Album>("$filter=Id eq 1");

            Assert.IsNotNull(q);
        }

        [Test]
        public void CreateODataQuery_WhenExistingFilterIsModified_ReturnsQuery()
        {
            var q = ODataQuery.Create<Album>("$filter=Id le 10&$orderby=Id&$top=10&$skip=5");
            var m = q.FilterExpression.And("contains(tolower(Title), 'rock')").Or("contains(tolower(Title), 'hell')").GetQuery();

            Assert.AreEqual(q.Options.Top.RawValue, m.Options.Top.RawValue);
            Assert.AreEqual(q.Options.Skip.RawValue, m.Options.Skip.RawValue);
            Assert.AreEqual(q.Options.OrderBy.RawValue, m.Options.OrderBy.RawValue);
            Assert.AreEqual("Id le 10 and contains(tolower(Title), 'rock') or contains(tolower(Title), 'hell')", m.Options.Filter.RawValue);
        }

        [Test]
        public void CreateODataQuery_WhenEmptyFilterIsModifiedWithAndFirst_ReturnsQuery()
        {
            var q = ODataQuery.Create<Album>("$orderby=Id&$top=10&$skip=5");
            var m = q.FilterExpression.And("contains(tolower(Title), 'rock')").Or("contains(tolower(Title), 'hell')").GetQuery();

            Assert.AreEqual(q.Options.Top.RawValue, m.Options.Top.RawValue);
            Assert.AreEqual(q.Options.Skip.RawValue, m.Options.Skip.RawValue);
            Assert.AreEqual(q.Options.OrderBy.RawValue, m.Options.OrderBy.RawValue);
            Assert.AreEqual("contains(tolower(Title), 'rock') or contains(tolower(Title), 'hell')", m.Options.Filter.RawValue);
        }

        [Test]
        public void CreateODataQuery_WhenEmptyFilterIsModifiedWithOrFirst_ReturnsQuery()
        {
            var q = ODataQuery.Create<Album>("$orderby=Id&$top=10&$skip=5");
            var m = q.FilterExpression.Or("contains(tolower(Title), 'hell')").And("contains(tolower(Title), 'rock')").GetQuery();

            Assert.AreEqual(q.Options.Top.RawValue, m.Options.Top.RawValue);
            Assert.AreEqual(q.Options.Skip.RawValue, m.Options.Skip.RawValue);
            Assert.AreEqual(q.Options.OrderBy.RawValue, m.Options.OrderBy.RawValue);
            Assert.AreEqual("contains(tolower(Title), 'hell') and contains(tolower(Title), 'rock')", m.Options.Filter.RawValue);
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
                                        { "odata.QuerySettings", new ODataQuerySettings() { PageSize = 1 } },
                                        { HttpPropertyKeys.HttpConfigurationKey, this.httpConfiguration }
                                }
                        });

            var p = q.ApplyTo(c);

            Assert.AreEqual(2, p.Count);
            Assert.IsNotNull(p.NextLink);
            Assert.AreEqual(1, p.Count());
        }

        [TestCase("$filter=tolower(Name) eq 'Test & test'")]
        [TestCase("$filter=tolower(Name) eq 'Test & test'&$orderby=Name")]
        public void CreateOptions_WhenGivenValidQuery_ReturnsValidQueryOptions(string query)
        {
            var f = new ODataQueryFactory();
            var options = f.CreateOptions<Album>(query, ODataQueryMapper.Engine.Configuration);

            Console.WriteLine($"Original: {query}");
            Console.WriteLine($"Created: {options.ToODataUriString()}");

            Assert.AreEqual(query, options.ToODataUriString());
        }

        [TestCase("$filter=tolower(Name) eq 'Test & test'")]
        [TestCase("$filter=tolower(Name) eq 'Test & test'&$orderby=Name")]
        public void Map_WhenGivenQuery_ReturnsSameQuery(string query)
        {
            var f = new ODataQueryFactory();
            var options = f.CreateOptions<Album>(query, ODataQueryMapper.Engine.Configuration);

            var mappedQuery = ODataQueryMapper.Engine.Map<Album, DomainAlbum>(options);

            Console.WriteLine($"Original: {options.ToODataUriString()}");
            Console.WriteLine($"Mapped: {mappedQuery.RawValue}");

            Assert.AreEqual(query, mappedQuery.RawValue);
        }

        [Test]
        public void Modify_WhenGivenQuery_ReturnsModifiedQuery()
        {
            var f = new ODataQueryFactory();
            var options = f.CreateOptions<Album>("$filter=tolower(Name) eq 'test & test'", ODataQueryMapper.Engine.Configuration);

            var mappedQuery = new ODataQuery<Album>(options);
            var modifiedQuery = f.Modify(new Dictionary<string, string>()
            {
                { "$orderby", "Name" }
            }, mappedQuery);

            Console.WriteLine($"Original: {mappedQuery.RawValue}");
            Console.WriteLine($"Mapped: {modifiedQuery.RawValue}");

            Assert.AreEqual("$filter=tolower(Name) eq 'test & test'&$orderby=Name", modifiedQuery.RawValue);
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