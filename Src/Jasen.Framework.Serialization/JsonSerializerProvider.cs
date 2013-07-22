using System; 
using Newtonsoft.Json; 

namespace Jasen.Framework.Serialization
{
    public class JsonSerializerProvider
    {
        public static T Deserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default(T);
            }
        }

        public static bool Deserialize<T>(string json, out T entity)
        {
            entity = default(T);

            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            try
            {
                entity = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Deserialize<T>(string json, out T entity, ref string error)
        {
            entity = default(T);

            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            try
            {

                entity = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch (Exception ex)
            {
                error += ex.Message;
                return false;
            }
        }
         
        public static string Serialize(object entity)
        {
            return Serialize(entity, false);
        }
         
        public static string Serialize(object entity, bool isIgnoreNullValue)
        {
            if (entity == null)
            {
                return string.Empty;
            }
            try
            {
                if (isIgnoreNullValue)
                {
                    var s = new JsonSerializerSettings();
                    s.NullValueHandling = NullValueHandling.Ignore;
                    return JsonConvert.SerializeObject(entity, Formatting.None, s);
                }
                else
                {
                    return JsonConvert.SerializeObject(entity);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

       
    }
}
