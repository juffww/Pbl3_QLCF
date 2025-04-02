using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;

namespace pbl3_QLCF
{
    public static class SessionExtensions
    {
        //This method serializes an object to JSON and stores it in the session as a string.

        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        //string key: The key corresponding to the JSON object stored in the session.
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}

