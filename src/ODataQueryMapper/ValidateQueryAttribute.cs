namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using System.Web.OData;
    using System.Web.OData.Extensions;
    using System.Web.OData.Query;

    /// <summary>An attribute that only validates OData queries, and leaves the processing to others.</summary>
    public class ValidateQueryAttribute : EnableQueryAttribute
    {
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            // Store validation settings on request
            actionContext.Request.Properties.Add("odata.ValidationSettings", this.GetValidationSettings());
            actionContext.Request.Properties.Add("odata.QuerySettings", this.GetQuerySettings());

            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        public override IQueryable ApplyQuery(IQueryable queryable, ODataQueryOptions queryOptions)
        {
            return queryable;
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var request = actionExecutedContext.Request;
            var response = actionExecutedContext.Response;

            if (response?.Content == null || !response.IsSuccessStatusCode)
            {
                return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
            }

            var objectContent = response.Content as ObjectContent;
            if (objectContent == null)
            {
                return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
            }

            var data = objectContent.Value as IODataCollection;
            if (data != null && request.ODataProperties().Path != null)
            {
                request.ODataProperties().TotalCount = data.Count;

                Uri nextLink;
                if (this.GetQuerySettings().PageSize.HasValue && Uri.TryCreate(data.NextLink, UriKind.Absolute, out nextLink))
                {
                    request.ODataProperties().NextLink = nextLink;
                }
            }

            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }

        private ODataQuerySettings GetQuerySettings()
        {
            var settings = new ODataQuerySettings()
            {
                EnableConstantParameterization = this.EnableConstantParameterization,
                EnsureStableOrdering = this.EnsureStableOrdering,
                HandleNullPropagation = this.HandleNullPropagation,
                PageSize = this.PageSize > 0 ? this.PageSize : (int?)null
            };

            return settings;
        }

        private ODataValidationSettings GetValidationSettings()
        {
            var settings = new ODataValidationSettings()
            {
                AllowedQueryOptions = this.AllowedQueryOptions,
                AllowedArithmeticOperators = this.AllowedArithmeticOperators,
                AllowedFunctions = this.AllowedFunctions,
                AllowedLogicalOperators = this.AllowedLogicalOperators,
                MaxAnyAllExpressionDepth = this.MaxAnyAllExpressionDepth,
                MaxExpansionDepth = this.MaxExpansionDepth,
                MaxNodeCount = this.MaxNodeCount,
                MaxOrderByNodeCount = this.MaxOrderByNodeCount,
                MaxSkip = this.MaxSkip > 0 ? this.MaxSkip : (int?)null,
                MaxTop = this.MaxTop > 0 ? this.MaxTop : (int?)null
            };

            if (!string.IsNullOrEmpty(this.AllowedOrderByProperties))
            {
                foreach (var s in this.AllowedOrderByProperties.Split(','))
                {
                    settings.AllowedOrderByProperties.Add(s);
                }
            }

            return settings;
        }
    }
}