namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using Microsoft.Owin.Testing;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        [TestCase("$top=10&$count=true")]
        public async Task GetAlbums_WhenGivenODataQuery_ReturnsAlbums(string query)
        {
            var response = await this.server.HttpClient.GetAsync($"odata/album?{query}");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<ODataCollection<DomainAlbum>>(content);

            Assert.Greater(result.Count, result.Values.Count(), "The API returned the wrong count");
            Assert.IsTrue(result.Values.Any(), "API returned no items");
        }

        [TestCase("$top=10&$count=true")]
        public async Task GetArtists_WhenGivenODataQuery_ReturnsArtists(string query)
        {
            var response = await this.server.HttpClient.GetAsync($"odata/artist?{query}");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<IEnumerable<DomainArtist>>(content);

            Assert.IsTrue(result.Any(), "API returned no items");
        }
    }
}