namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces
{
    using Microsoft.OData.Edm;
    using System;

    public interface IConfiguration : IProfileConfiguration
    {
        /// <summary>Gets a value indicating whether the sealed.</summary>
        /// <value>true if sealed, false if not.</value>
        bool Sealed { get; }

        /// <summary>Gets the initialization expression.</summary>
        /// <value>The initialization expression.</value>
        Action<IConfiguration> InitializationExpression { get; }

        /// <summary>Gets the model.</summary>
        /// <value>The model.</value>
        IEdmModel Model { get; }

        /// <summary>Adds the specified profile.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        void AddProfile<T>() where T : IMappingProfile;
    }
}