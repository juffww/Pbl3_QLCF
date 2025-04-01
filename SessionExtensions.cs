using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace pbl3_QLCF
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
        //Make sure the ChiTietDonHangs list is properly serialized and deserialized
        //public static void SetObjectAsJson(this ISession session, string key, object value)
        //{
        //    var options = new JsonSerializerOptions
        //    {
        //        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
        //        PropertyNameCaseInsensitive = true
        //    };
        //    session.SetString(key, JsonSerializer.Serialize(value, options));
        //}

        //public static T GetObjectFromJson<T>(this ISession session, string key)
        //{
        //    var value = session.GetString(key);
        //    if (value == null)
        //        return default;

        //    var options = new JsonSerializerOptions
        //    {
        //        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
        //        PropertyNameCaseInsensitive = true
        //    };
        //    return JsonSerializer.Deserialize<T>(value, options);
        //}
    }
}

