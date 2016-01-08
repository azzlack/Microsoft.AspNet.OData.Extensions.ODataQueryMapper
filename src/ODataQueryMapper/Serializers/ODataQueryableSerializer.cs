namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Serializers
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Linq;

    public class ODataQueryableSerializer : ODataCollectionSerializer
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param><param name="value">The value.</param><param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var collection = value as IODataCollection;

            if (collection == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            // Count
            writer.WritePropertyName("@odata.count");
            writer.WriteValue(collection.Count);

            // Value
            writer.WritePropertyName("value");

            var list = value as IEnumerable;
            if (list == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteStartArray();

                foreach (var item in list)
                {
                    serializer.Serialize(writer, item);
                }

                writer.WriteEndArray();
            }

            // NextLink
            if (!string.IsNullOrEmpty(collection.NextLink))
            {
                writer.WritePropertyName("@odata.nextLink");
                writer.WriteValue(collection.NextLink);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IODataQueryable<>));
        }
    }
}