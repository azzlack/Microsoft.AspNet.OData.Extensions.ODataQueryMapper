namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using Microsoft.Owin.Testing;

    using Newtonsoft.Json;

    using NUnit.Framework;

    [TestFixture]
    public class ApiTests
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

        [Test]
        public async Task GetAlbums_WhenUsingNormalApi_ReturnsAlbums()
        {
            var response = await this.server.HttpClient.GetAsync("api/album");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<IEnumerable<DomainAlbum>>(content);

            Assert.IsTrue(result.Any(), "API returned no items");
        }

        [Test]
        public async Task GetArtists_WhenUsingNormalApi_ReturnsArtists()
        {
            var response = await this.server.HttpClient.GetAsync("api/artist");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<IEnumerable<DomainArtist>>(content);

            Assert.IsTrue(result.Any(), "API returned no items");
        }
    }
}