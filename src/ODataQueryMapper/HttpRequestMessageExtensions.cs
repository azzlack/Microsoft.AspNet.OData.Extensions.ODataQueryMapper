namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using System.Net.Http;
    using System.Web.OData.Query;

    public static class HttpRequestMessageExtensions
    {
        public static ODataValidationSettings ODataValidationSettings(this HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("odata.ValidationSettings"))
            {
                return request.Properties["odata.ValidationSettings"] as ODataValidationSettings;
            }

            return null;
        }
        public static ODataQuerySettings ODataQuerySettings(this HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("odata.QuerySettings"))
            {
                return request.Properties["odata.QuerySettings"] as ODataQuerySettings;
            }

            return null;
        }
    }
}