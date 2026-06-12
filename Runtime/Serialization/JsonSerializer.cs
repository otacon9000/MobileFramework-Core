using System;
using Newtonsoft.Json;

namespace MobileFramework.Core.Serialization
{
    /// <summary>
    /// Unico punto di accesso alla serializzazione JSON del framework.
    /// Wrappa Newtonsoft Json.NET: nessun altro file deve usare JsonConvert
    /// o JsonUtility direttamente.
    /// </summary>
    public static class JsonSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Include
        };

        public static string Serialize(object value, bool prettyPrint = false)
        {
            return JsonConvert.SerializeObject(value, prettyPrint ? Formatting.Indented : Formatting.None, Settings);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        public static object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, Settings);
        }

        /// <summary>Popola un'istanza esistente: i campi assenti nel JSON mantengono il valore corrente.</summary>
        public static void Populate(string json, object target)
        {
            JsonConvert.PopulateObject(json, target, Settings);
        }

        public static bool TryDeserialize<T>(string json, out T result)
        {
            try
            {
                result = Deserialize<T>(json);
                return result != null;
            }
            catch (JsonException)
            {
                result = default;
                return false;
            }
        }
    }
}
