namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Serializers
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Linq;

    public class ODataCollectionSerializer : JsonConverter
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
            serializer.Serialize(writer, collection.GetValue());

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
            return objectType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IODataCollection<>));
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param><param name="objectType">Type of the object.</param><param name="existingValue">The existing value of object being read.</param><param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var obj = JObject.Load(reader);

            if (obj.HasValues)
            {
                var count = obj.Value<int>("@odata.count");
                var nextLink = obj.Value<string>("@odata.nextLink");
                var value = obj.Value<JArray>("value");

                var result = (IODataCollection)Activator.CreateInstance(objectType);
                result.Initialize(value, count, nextLink);

                return result;
            }

            return null;
        }
    }
}