namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    public interface IMappingProfile
    {
        /// <summary>Method for configuring mappers.</summary>
        /// <param name="configuration">The configuration.</param>
        void Configure(IProfileConfiguration configuration);
    }
}