Microsoft.AspNet.OData.Extensions.ODataQueryMapper
-------------------------------------------
A query mapper for OData v4.0, Automapper style. This extension allows you to have a different domain model than your database model, and still be able to pass the OData query down to the database.

## Basic Usage
The following example uses the `Album` table from the [Chinook](http://chinookdatabase.codeplex.com/) database.  

Initialize the mapper in your `Startup.cs` file:

```csharp
ODataQueryMapper.Initialize(
    x =>
        {
            x.CreateMap<DomainAlbum, Album>("album")
                .ForMember(y => y.Id, y => y.AlbumId);
        });
```
(The `CreateMap` function creates the necessary code for your `ODataController` to work)

Next, create a controller that gets and transforms our data:

```csharp
[ODataRoutePrefix("album")] // NOTE: This must be the same as the one registered in the initializer
public class AlbumODataController : ODataController
{
    private readonly IODataQueryMapper queryMapper;

    private readonly IMappingEngine mapper;

    public AlbumODataController(IODataQueryMapper queryMapper, IMappingEngine mapper)
    {
        this.queryMapper = queryMapper;
        this.mapper = mapper;
    }

    [ODataRoute]
    public async Task<IHttpActionResult> Get(ODataQueryOptions<DomainAlbum> query)
    {
        var mappedQuery = await this.queryMapper.Map<DomainAlbum, Album>(query);

        using (var context = new MyDatabaseContext()) // Using EF here, but other frameworks can be used
        {
            var dbTesult = mappedQuery.ApplyTo(context.Album.Include(x => x.Artist))

            // The database entities must be mapped back to your domain models, this example uses Automapper
            var result = this.mapper.Map<IEnumerable<DomainAlbum>>(dbResult);

            if (result != null && result.Any())
            {
                return this.Ok(result);
            }
        }

        return this.StatusCode(HttpStatusCode.NoContent);
    }
}
```

# Advanced Usage
The `ForMember` method also accepts strings:

```csharp
x.CreateMap<DomainAlbum, Album>("album")
    .ForMember("Artist/Value", y => y.ArtistId);
x.CreateMap<DomainAlbum, Album>("album")
    .ForMember("Artist/Value", "ArtistId");
```

### Profiles
It is possible to create mapping profiles as separate classes, letting you better have control over your code:

```csharp
x.AddProfile<MyProfile>();

public class MyProfile : IMappingProfile
{
    public void Configure(IProfileConfiguration configuration)
    {
        configuration.CreateMap<DomainArtist, Artist>("artist").ConvertUsing<ArtistConverter>();
    }
}
```

### TypeConverters
It is also possible to create mapping tables as separate classes, letting you better have control over your code:

```csharp
x.CreateMap<DomainArtist, Artist>("artist").ConvertUsing<MyConverter>();

public class MyConverter : ITypeConverter<DomainArtist, Artist>
{
    public Dictionary<string, string> CreateMappingTable()
    {
        return new Dictionary<string, string>()
                   {
                       { "Id", "ArtistId" }
                   };
    }
}
```

# Important Notes
### EnableQueryAttribute
You cannot use the `[EnableQuery]` attribute, as that will process your query twice, once in the `ApplyTo` call, and once when the request is leaving the server. Use the included `[ValidateQuery]` attribute instead. It takes care of validating the query according to your supplied settings, and does not process the result after `ApplyTo` has been called.

### Nested properties
If you want to perform filtering on a nested property on your domain model, like `Artist.Name`, you need to use the string version of `ForMember` like this:

```csharp
x.CreateMap<DomainAlbum, Album>("album")
    .ForMember("Artist/Value", "ArtistId");
```  
