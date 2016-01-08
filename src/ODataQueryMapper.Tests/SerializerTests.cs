namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Serializers;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Profiles;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class SerializerTests
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
        public void CanConvert_WhenGivenODataQueryable_ReturnsTrue()
        {
            var s = new ODataQueryableSerializer();

            Assert.IsTrue(s.CanConvert(typeof(ODataQueryable<Album>)));
        }

        [Test]
        public void Serialize_WhenGivenODataQueryable_ReturnsCorrectJson()
        {
            var s =
                new ODataQueryable<Album>(
                    new List<Album>() { new Album() { AlbumId = 1, Title = "Test" } }.AsQueryable(),
                    1);
            s.NextLink = "http://localhost/odata/album?$top=50&$skip=50";

            var r = JsonConvert.SerializeObject(s);

            Console.WriteLine(r);

            Assert.AreEqual("{\"@odata.count\":1,\"value\":[{\"AlbumId\":1,\"Title\":\"Test\",\"ArtistId\":0,\"Artist\":null,\"Track\":[]}],\"@odata.nextLink\":\"http://localhost/odata/album?$top=50&$skip=50\"}", r);
        }

        [Test]
        public void Deserialize_WhenGivenJson_ReturnsODataQueryable()
        {
            var json = "{\"@odata.count\":1,\"value\":[{\"AlbumId\":1,\"Title\":\"Test\",\"ArtistId\":0,\"Artist\":null,\"Track\":[]}],\"@odata.nextLink\":\"http://localhost/odata/album?$top=50&$skip=50\"}";

            var r1 = JsonConvert.DeserializeObject<ODataQueryable<Album>>(json);
            var r2 = JsonConvert.SerializeObject(r1);

            Assert.AreEqual(json, r2);
        }

        [Test]
        public void Deserialize_WhenGivenJson_ReturnsODataCollection()
        {
            var json = "{\"@odata.count\":1,\"value\":[{\"AlbumId\":1,\"Title\":\"Test\",\"ArtistId\":0,\"Artist\":null,\"Track\":[]}],\"@odata.nextLink\":\"http://localhost/odata/album?$top=50&$skip=50\"}";

            var r1 = JsonConvert.DeserializeObject<ODataCollection<Album>>(json);
            var r2 = JsonConvert.SerializeObject(r1);

            Assert.AreEqual(json, r2);
        }
    }
}