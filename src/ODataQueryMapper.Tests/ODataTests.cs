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
        [TestCase("$filter=contains(tolower(Title), 'rock')&$orderby=Title")]
        public async Task GetAlbums_WhenGivenSimpleODataQuery_ReturnsAlbums(string query)
        {
            var response = await this.server.HttpClient.GetAsync($"odata/album?{query}");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<ODataCollection<DomainAlbum>>(content);

            Assert.IsTrue(result.Value.Any(), "API returned no items");
        }

        [Test]
        public async Task GetAlbums_WhenGivenPagedODataQuery_ReturnsPagedAlbums()
        {
            var response = await this.server.HttpClient.GetAsync($"odata/album?$top=100");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<ODataCollection<DomainAlbum>>(content);

            Assert.AreEqual(50, result.Value.Count(), "Wrong number of items returned");
            Assert.AreEqual("http://localhost/odata/album?$top=50&$skip=50", result.NextLink, "NextLink is wrong");
        }

        [Test]
        public async Task GetAlbums_WhenGivenPagedODataQuery_ReturnsCorrectNextLink()
        {
            var response = await this.server.HttpClient.GetAsync($"odata/album?$top=300&$skip=100");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<ODataCollection<DomainAlbum>>(content);

            Assert.AreEqual($"http://localhost/odata/album?$top=250&$skip=150", result.NextLink, "NextLink is wrong");
        }

        [Test]
        public async Task GetAlbums_WhenGivenFilteredODataQuery_ReturnsAlbums()
        {
            var response = await this.server.HttpClient.GetAsync($"odata/album?$filter=Id eq 1");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<ODataCollection<DomainAlbum>>(content);

            Assert.AreEqual(1, result.Value.First().Id);
            Assert.AreEqual(1, result.Value.Count(), "Wrong number of items returned");
        }

        [Test]
        public async Task GetAlbums_WhenGivenSelectODataQuery_ReturnsException()
        {
            var response = await this.server.HttpClient.GetAsync($"odata/album?$select=Title");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [TestCase("$top=10&$skip=10&")]
        [TestCase("$top=10&$skip=10&$count=true")]
        [TestCase("$top=50&$skip=100")]
        [TestCase("$top=50&$skip=100&$count=true")]
        public async void GetAlbums_WhenGivenTopAndSkipLowerThanPageSize_ReturnsNoNextLink(string query)
        {
            var getResponse = await this.server.HttpClient.GetAsync($"odata/album?{query}");
            var content = await getResponse.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<ODataCollection<DomainAlbum>>(content);

            Assert.IsNullOrEmpty(data.NextLink);
        }

        [TestCase("$top=100&$skip=10&")]
        [TestCase("$top=100&$skip=10&$count=true")]
        [TestCase("$top=500&$skip=100")]
        [TestCase("$top=500&$skip=100&$count=true")]
        public async void GetAlbums_WhenGivenTopAndSkipLargerThanPageSize_ReturnsNextLink(string query)
        {
            var getResponse = await this.server.HttpClient.GetAsync($"odata/album?{query}");
            var content = await getResponse.Content.ReadAsStringAsync();

            var data = JsonConvert.DeserializeObject<ODataCollection<DomainAlbum>>(content);

            Console.WriteLine($"NextLink: {data.NextLink}");

            Assert.IsNotNullOrEmpty(data.NextLink);
        }

        [TestCase("")]
        [TestCase("$top=10&$count=true")]
        [TestCase("$top=10")]
        [TestCase("$top=50&$skip=50")]
        [TestCase("$filter=contains(tolower(DisplayName), 'roses')&$orderby=DisplayName")]
        public async Task GetArtists_WhenGivenSimpleODataQuery_ReturnsArtists(string query)
        {
            var response = await this.server.HttpClient.GetAsync($"odata/artist?{query}");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<ODataCollection<DomainArtist>>(content);

            Assert.IsTrue(result.Value.Any(), "API returned no items");
        }

        [Test]
        public async Task GetArtists_WhenGivenPagedODataQuery_ReturnsArtists()
        {
            var response = await this.server.HttpClient.GetAsync($"odata/artist?$top=100");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<ODataCollection<DomainAlbum>>(content);

            Assert.AreEqual(50, result.Value.Count(), "Wrong number of items returned");
            Assert.AreEqual("http://localhost/odata/artist?$top=50&$skip=50", result.NextLink, "NextLink is wrong");
        }

        [Test]
        public async Task GetArtists_WhenGivenFilteredODataQuery_ReturnsArtists()
        {
            var response = await this.server.HttpClient.GetAsync($"odata/artist?$filter=Id eq 1");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<ODataCollection<DomainAlbum>>(content);

            Assert.AreEqual(1, result.Value.First().Id);
            Assert.AreEqual(1, result.Value.Count(), "Wrong number of items returned");
        }

        [Test]
        public async Task GetArtists_WhenGivenSelectODataQuery_ReturnsException()
        {
            var response = await this.server.HttpClient.GetAsync($"odata/artist?$select=Title");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Test]
        public async Task GetTracks_WhenGivenPagedODataQuery_ReturnsTracks()
        {
            var response = await this.server.HttpClient.GetAsync($"odata/track?$top=100");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<ODataCollection<DomainTrack>>(content);

            Assert.AreEqual(100, result.Value.Count(), "Wrong number of items returned");
            Assert.AreEqual(null, result.NextLink, "NextLink is wrong");
        }

        [Test]
        public async Task GetTrack_WhenGivenTrackId_ReturnsTrack()
        {
            var response = await this.server.HttpClient.GetAsync($"odata/track(1)");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<DomainTrack>(content);

            Assert.AreEqual(1, result.Wbs);
        }

        [TestCase(1)]
        [TestCase(100)]
        public async Task GetTracks_WhenGivenAlbumId_ReturnsAlbumTracks(long albumId)
        {
            var response = await this.server.HttpClient.GetAsync($"odata/track?$filter=Album/Value eq {albumId}");
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            var result = JsonConvert.DeserializeObject<ODataCollection<DomainTrack>>(content);

            Assert.IsTrue(result.Value.All(x => x.Album.Value == albumId), "The tracks have wrong album");
        }
    }
}