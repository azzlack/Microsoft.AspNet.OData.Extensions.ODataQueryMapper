namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using System;
    using System.Web.Http.Dependencies;
    using System.Web.Http.Dispatcher;
    using System.Web.OData.Query;
    using System.Web.OData.Query.Expressions;
    using System.Web.OData.Query.Validators;

    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OData.Edm;

    public class ODataServiceProvider : IServiceProvider
    {
        private readonly IConfiguration configuration;

        public ODataServiceProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                if (serviceType.IsAssignableFrom(typeof(IServiceScopeFactory)))
                {
                    return new ODataServiceScopeFactory(this.configuration);
                }

                if (serviceType.IsAssignableFrom(typeof(FilterQueryValidator)))
                {
                    return new FilterQueryValidator(new DefaultQuerySettings());
                }

                if (serviceType.IsAssignableFrom(typeof(OrderByQueryValidator)))
                {
                    return new OrderByQueryValidator(new DefaultQuerySettings());
                }

                if (serviceType.IsAssignableFrom(typeof(SelectExpandQueryValidator)))
                {
                    return new SelectExpandQueryValidator(new DefaultQuerySettings());
                }

                if (serviceType.IsAssignableFrom(typeof(CountQueryValidator)))
                {
                    return new CountQueryValidator(new DefaultQuerySettings());
                }

                if (serviceType.IsAssignableFrom(typeof(TopQueryValidator)))
                {
                    return new TopQueryValidator();
                }

                if (serviceType.IsAssignableFrom(typeof(SkipQueryValidator)))
                {
                    return new SkipQueryValidator();
                }

                if (serviceType.IsAssignableFrom(typeof(FilterBinder)))
                {
                    return new FilterBinder(this);
                }

                if (serviceType.IsAssignableFrom(typeof(IEdmModel)))
                {
                    return this.configuration.Model;
                }

                if (serviceType.IsAssignableFrom(typeof(IAssembliesResolver)))
                {
                    return new DefaultAssembliesResolver();
                }

                return Activator.CreateInstance(serviceType);
            }
            catch
            {
                return null;
            }
        }
    }

    public class ODataServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IConfiguration configuration;

        public ODataServiceScopeFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IServiceScope CreateScope()
        {
            return new ODataServiceScope(new ODataServiceProvider(this.configuration));
        }
    }

    public class ODataServiceScope : IServiceScope
    {
        private IServiceProvider serviceProvider;

        public ODataServiceScope(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Dispose()
        {
            this.serviceProvider = null;
        }

        public IServiceProvider ServiceProvider => this.serviceProvider;
    }
}