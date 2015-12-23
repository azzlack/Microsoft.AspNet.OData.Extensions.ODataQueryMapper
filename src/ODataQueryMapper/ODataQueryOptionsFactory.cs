namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.OData.Edm;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web.OData;

    public class ODataQueryOptionsFactory : IODataQueryOptionsFactory
    {
        /// <summary>Creates a new ODataQueryOptions{T}</summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="clauses">The clauses.</param>
        /// <param name="context">The context.</param>
        /// <param name="request">The request.</param>
        /// <returns>An OData query object.</returns>
        public IODataQuery<T> Create<T>(
            Dictionary<string, string> clauses,
            ODataQueryContext context,
            HttpRequestMessage request)
        {
            var builder = new UriBuilder(request.RequestUri);

            // Replace query string items
            var querystring = this.GetQueryParameters(request, context);

            if (querystring.ContainsKey("$filter") && clauses.ContainsKey("$filter"))
            {
                querystring["$filter"] = clauses["$filter"];
            }

            if (querystring.ContainsKey("$orderby") && clauses.ContainsKey("$orderby"))
            {
                querystring["$orderby"] = clauses["$orderby"];
            }

            if (querystring.ContainsKey("$expand") && clauses.ContainsKey("$expand"))
            {
                querystring["$expand"] = clauses["$expand"];
            }

            if (querystring.ContainsKey("$select") && clauses.ContainsKey("$select"))
            {
                querystring["$select"] = clauses["$select"];
            }

            request.RequestUri = builder.Uri;

            return new ODataQuery<T>(context, request);
        }

        private IDictionary<string, string> GetQueryParameters(HttpRequestMessage request, ODataQueryContext context)
        {
            var querystring = request.GetQueryNameValuePairs().ToDictionary(p => p.Key, p => p.Value);

            var type = context.ElementType as IEdmEntityType;

            if (type != null && querystring.ContainsKey("$select"))
            {
                var enumerable = type.NavigationProperties();
                if (enumerable != null)
                {
                    var str = string.Empty;
                    foreach (var navigationProperty in enumerable)
                    {
                        if (this.GetPropertyRestrictions(navigationProperty, context.Model).Restrictions.AutoExpand)
                        {
                            if (!string.IsNullOrEmpty(str))
                            {
                                str += ",";
                            }

                            str += navigationProperty.Name;
                        }
                    }

                    if (!string.IsNullOrEmpty(str))
                    {
                        if (querystring.ContainsKey("$expand"))
                        {
                            IDictionary<string, string> dictionary2;
                            (dictionary2 = querystring)["$expand"] = dictionary2["$expand"] + "," + str;
                        }
                        else
                        {
                            querystring["$expand"] = str;
                        }
                    }
                }
            }

            return querystring;
        }

        private QueryableRestrictionsAnnotation GetPropertyRestrictions(IEdmProperty edmProperty, IEdmModel edmModel)
        {
            return edmModel.GetAnnotationValue<QueryableRestrictionsAnnotation>(edmProperty);
        }
    }
}