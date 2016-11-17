namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests
{
    using AutoMapper;
    using global::Owin;
    using HibernatingRhinos.Profiler.Appender.EntityFramework;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Functions;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Interfaces;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Models;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Profiles;
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Tests.Repositories;
    using SimpleInjector;
    using SimpleInjector.Integration.WebApi;
    using System.Web.Http;
    using System.Web.OData.Extensions;
    using System.Web.OData.Query;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            EntityFrameworkProfiler.Initialize();

            // Create DI container
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();

            // Create WebAPI config
            var config = new HttpConfiguration();

            // Set up DI container
            container.RegisterWebApiControllers(config);

            // Set up ODataQueryMapper
            ODataQueryMapper.Initialize(
                x =>
                    {
                        x.CreateMap<DomainAlbum, Album>("album")
                            .ForMember(y => y.Id, y => y.AlbumId);
                        x.Configure<Album>("album2", y => y.HasKey(z => z.AlbumId).Count().Filter().OrderBy(), true);

                        x.AddProfile<TestProfile>();
                    });

            // Set up Automapper
            Mapper.Initialize(
               x =>
               {
                   x.ConstructServicesUsing(config.DependencyResolver.GetService);
                   x.CreateMap<Album, DomainAlbum>()
                       .ForMember(m => m.Id, y => y.MapFrom(z => z.AlbumId))
                       .ForMember(m => m.Artist, y => y.MapFrom(z => new Resource<long>() { Value = z.Artist.ArtistId, Title = z.Artist.Name, Link = $"/api/artist/{z.Artist.ArtistId}" }));
                   x.CreateMap<Artist, DomainArtist>()
                       .ForMember(m => m.Id, y => y.MapFrom(z => z.ArtistId))
                       .ForMember(m => m.DisplayName, y => y.MapFrom(z => z.Name))
                       .ForMember(m => m.Albums, y => y.Ignore());
                   x.CreateMap<Track, DomainTrack>()
                       .ForMember(m => m.Wbs, y => y.MapFrom(z => z.TrackId))
                       .ForMember(m => m.Title, y => y.MapFrom(z => z.Name))
                       .ForMember(m => m.Album, y => y.MapFrom(z => new Resource<long>() { Value = z.Album.AlbumId, Title = z.Album.Title, Link = $"/api/album/{z.Album.AlbumId}" }));
               });

            container.RegisterWebApiRequest(() => ODataQueryMapper.Engine);
            container.RegisterWebApiRequest(() => Mapper.Instance);
            container.RegisterWebApiRequest<IAlbumRepository, AlbumRepository>();
            container.RegisterWebApiRequest<IArtistRepository, ArtistRepository>();
            container.RegisterWebApiRequest<ITrackRepository, TrackRepository>();
            container.RegisterWebApiRequest<IArtistFunctions, ArtistFunctions>();
            container.RegisterWebApiRequest<IAlbumFunctions, AlbumFunctions>();
            container.RegisterWebApiRequest<ITrackFunctions, TrackFunctions>();

            container.Verify();

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });
            config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
            config.MapODataServiceRoute(
                "DefaultOData",
                "odata",
                ODataQueryMapper.Engine.GetModel());

            // Register the dependency resolver.
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            config.EnsureInitialized();

            // Start WebAPI
            app.UseWebApi(config);
        }
    }
}