using Newtonsoft.Json;

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

            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            var serializeSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source, serializeSettings), deserializeSettings);
        }
    }
}