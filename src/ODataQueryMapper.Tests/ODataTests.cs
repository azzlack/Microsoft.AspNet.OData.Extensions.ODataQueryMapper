namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using Microsoft.Owin.Testing;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    [TestFixture]
    public class ODataTests
    {
        private TestServer server;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.server = TestServer.Create<Startup>();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            this.server.Dispose();
        }

        [TestCase("")]
        [TestCase("$top=10&$count=true")]
        [TestCase("$top=10")]
        [TestCase("$top=50&$skip=50")]
        public async Task GetAlbums_WhenGivenSimpleODataQuery_ReturnsAlbums(string query)
        {
            var response = await this.server.HttpClient.GetAsync($"odata/album?{query}");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<ODataCollection<DomainAlbum>>(content);

            Assert.IsTrue(result.Values.Any(), "API returned no items");
        }

        [Test]
        public async Task GetAlbums_WhenGivenPagedODataQuery_ReturnsAlbums()
        {
            var response = await this.server.HttpClient.GetAsync($"odata/album?$top=100");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<ODataCollection<DomainAlbum>>(content);

            Assert.AreEqual(50, result.Values.Count(), "Wrong number of items returned");
            Assert.AreEqual("http://localhost/odata/album?$top=50&$skip=50", result.NextLink, "NextLink is wrong");
        }

        [Test]
        public async Task GetAlbums_WhenGivenFilteredODataQuery_ReturnsAlbums()
        {
            var response = await this.server.HttpClient.GetAsync($"odata/album?$filter=Id eq 1");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<ODataCollection<DomainAlbum>>(content);

            Assert.AreEqual(1, result.Values.First().Id);
            Assert.AreEqual(1, result.Values.Count(), "Wrong number of items returned");
        }

        [Test]
        public async Task GetAlbums_WhenGivenSelectODataQuery_ReturnsException()
        {
            var response = await this.server.HttpClient.GetAsync($"odata/album?$select=Title");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [TestCase("")]
        [TestCase("$top=10&$count=true")]
        [TestCase("$top=10")]
        [TestCase("$top=50&$skip=50")]
        public async Task GetArtists_WhenGivenSimpleODataQuery_ReturnsArtists(string query)
        {
            var response = await this.server.HttpClient.GetAsync($"odata/artist?{query}");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<ODataCollection<DomainArtist>>(content);

            Assert.IsTrue(result.Values.Any(), "API returned no items");
        }
    }
}