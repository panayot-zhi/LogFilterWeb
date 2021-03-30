using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LogFilterWeb.Utility
{
    public static class ObjectCloner
    {
        public static T JsonCopy<T>(T source)
        {
            // Don't serialize a null object,
            // simply return the default for that object
            if (object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            var deserializeSettings = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ContractResolver = new JsonCopyContractResolver()
            };

            var serializeSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new JsonCopyContractResolver()
            };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source, serializeSettings), deserializeSettings);
        }

        public class JsonCopyContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);
                property.Ignored = false; // magic
                return property;
            }
        }
    }
}