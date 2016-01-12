namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.OData.Edm;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
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

            builder.Query = string.Join("&", querystring.Select(x => $"{x.Key}={x.Value}"));

            var mappedRequest = this.Clone(request);
            mappedRequest.RequestUri = builder.Uri;

            return new ODataQuery<T>(context, mappedRequest);
        }

        /// <summary>Modifies the original query and returns a new one.</summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="clauses">The clauses.</param>
        /// <param name="original">The original query.</param>
        /// <returns>A new query.</returns>
        public IODataQuery<T> Modify<T>(Dictionary<string, string> clauses, IODataQuery<T> original)
        {
            var builder = new UriBuilder(original.Options.Request.RequestUri);

            // Modify query string items
            var querystring = this.GetQueryParameters(original.Options.Request, original.Options.Context);

            if (clauses.ContainsKey("$filter"))
            {
                if (!querystring.ContainsKey("$filter"))
                {
                    if (clauses["$filter"].Trim().StartsWith("and"))
                    {
                        querystring["$filter"] = clauses["$filter"].Trim().Substring(3).Trim();
                    }
                    else if (clauses["$filter"].Trim().StartsWith("or"))
                    {
                        querystring["$filter"] = clauses["$filter"].Trim().Substring(2).Trim();
                    }
                    else
                    {
                        querystring["$filter"] = clauses["$filter"];
                    }
                }
                else
                {
                    querystring["$filter"] = querystring["$filter"] + clauses["$filter"];
                }
            }

            if (clauses.ContainsKey("$orderby"))
            {
                if (!querystring.ContainsKey("$orderby"))
                {
                    querystring["$orderby"] = clauses["$orderby"].Trim().StartsWith(",") ? clauses["$orderby"].Trim().Substring(1) : clauses["$orderby"];
                }
                else
                {
                    querystring["$orderby"] = querystring["$orderby"] + clauses["$orderby"];
                }
            }

            if (clauses.ContainsKey("$expand"))
            {
                if (!querystring.ContainsKey("$expand"))
                {
                    querystring["$expand"] = clauses["$expand"].Trim().StartsWith(",") ? clauses["$expand"].Trim().Substring(1) : clauses["$expand"];
                }
                else
                {
                    querystring["$expand"] = querystring["$expand"] + clauses["$expand"];
                }
            }

            if (clauses.ContainsKey("$select"))
            {
                if (!querystring.ContainsKey("$select"))
                {
                    querystring["$select"] = clauses["$select"].Trim().StartsWith(",") ? clauses["$select"].Trim().Substring(1) : clauses["$select"];
                }
                else
                {
                    querystring["$select"] = querystring["$select"] + clauses["$select"];
                }
            }

            builder.Query = string.Join("&", querystring.Select(x => $"{x.Key}={x.Value}"));

            var mappedRequest = this.Clone(original.Options.Request);
            mappedRequest.RequestUri = builder.Uri;

            return new ODataQuery<T>(original.Options.Context, mappedRequest);
        }

        private HttpRequestMessage Clone(HttpRequestMessage req)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri)
            {
                Version = req.Version
            };

            // Clone the request content
            if (req.Content != null)
            {
                var ms = new MemoryStream();

                Task.Run(
                    async () =>
                        {
                            await req.Content.CopyToAsync(ms).ConfigureAwait(false);
                        }).GetAwaiter().GetResult();

                ms.Position = 0;
                clone.Content = new StreamContent(ms);

                if (req.Content.Headers != null)
                {
                    foreach (var header in req.Content.Headers)
                    {
                        clone.Content.Headers.Add(header.Key, header.Value);
                    }
                }
            }

            // Clone properties
            foreach (var property in req.Properties)
            {
                clone.Properties.Add(property);
            }

            // Clone headers
            foreach (var header in req.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
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
                        var propertyRestrictions = this.GetPropertyRestrictions(navigationProperty, context.Model);
                        if (propertyRestrictions != null && propertyRestrictions.Restrictions.AutoExpand)
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