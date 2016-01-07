namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.OData.Query;

    public static class ODataQueryOptionsExtensions
    {
        /// <summary>Checks if this instance is empty (no query).</summary>
        /// <param name="queryOptions">The queryOptions to act on.</param>
        /// <returns>true if empty, false if not.</returns>
        public static bool IsEmpty(this ODataQueryOptions queryOptions)
        {
            return
                queryOptions.RawValues.Count == null
                && queryOptions.RawValues.DeltaToken == null
                && queryOptions.RawValues.Expand == null
                && queryOptions.RawValues.Filter == null
                && queryOptions.RawValues.Format == null
                && queryOptions.RawValues.OrderBy == null
                && queryOptions.RawValues.Select == null
                && queryOptions.RawValues.Skip == null
                && queryOptions.RawValues.SkipToken == null
                && queryOptions.RawValues.Top == null;
        }

        /// <summary>Converts this instance to an OData URI string.</summary>
        /// <param name="queryOptions">The queryOptions to act on.</param>
        /// <returns>An OData URI string.</returns>
        public static string ToODataUriString(this ODataQueryOptions queryOptions)
        {
            var components = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(queryOptions.RawValues.Count))
            {
                components.Add("$count", queryOptions.RawValues.Count);
            }

            if (!string.IsNullOrEmpty(queryOptions.RawValues.DeltaToken))
            {
                components.Add("$deltatoken", queryOptions.RawValues.DeltaToken);
            }

            if (!string.IsNullOrEmpty(queryOptions.RawValues.Expand))
            {
                components.Add("$expand", queryOptions.RawValues.Expand);
            }

            if (!string.IsNullOrEmpty(queryOptions.RawValues.Filter))
            {
                components.Add("$filter", queryOptions.RawValues.Filter);
            }

            if (!string.IsNullOrEmpty(queryOptions.RawValues.Format))
            {
                components.Add("$format", queryOptions.RawValues.Format);
            }

            if (!string.IsNullOrEmpty(queryOptions.RawValues.OrderBy))
            {
                components.Add("$orderby", queryOptions.RawValues.OrderBy);
            }

            if (!string.IsNullOrEmpty(queryOptions.RawValues.Select))
            {
                components.Add("$select", queryOptions.RawValues.Select);
            }

            if (!string.IsNullOrEmpty(queryOptions.RawValues.Skip))
            {
                components.Add("$skip", queryOptions.RawValues.Skip);
            }

            if (!string.IsNullOrEmpty(queryOptions.RawValues.SkipToken))
            {
                components.Add("$skiptoken", queryOptions.RawValues.SkipToken);
            }

            if (!string.IsNullOrEmpty(queryOptions.RawValues.Top))
            {
                components.Add("$top", queryOptions.RawValues.Top);
            }

            return string.Join("&", components.Select(x => $"{x.Key}={x.Value}"));
        }
    }
}