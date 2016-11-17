namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using System;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Hosting;

    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Enumerations;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;

    internal static class HttpRequestMessageFactory
    {
        /// <summary>Creates a message.</summary>
        /// <param name="method">The method.</param>
        /// <param name="url">URL of the document.</param>
        /// <returns>The new message.</returns>
        public static HttpRequestMessage CreateMessage(HttpMethod method, string url, IConfiguration configuration)
        {
            return CreateMessage(method, new Uri(url), configuration);
        }

        /// <summary>Creates a message.</summary>
        /// <param name="method">The method.</param>
        /// <param name="uri">The resource URI.</param>
        /// <returns>The new message.</returns>
        public static HttpRequestMessage CreateMessage(HttpMethod method, Uri uri, IConfiguration configuration)
        {
            var m = new HttpRequestMessage(method, uri);

            var c = new HttpConfiguration();
            c.Properties.AddOrUpdate(ODataKeys.NonODataRootContainerKey, new ODataServiceProvider(configuration), updateValueFactory: (oldValue, newValue) => newValue);

            m.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, c);

            return m;
        }
    }
}