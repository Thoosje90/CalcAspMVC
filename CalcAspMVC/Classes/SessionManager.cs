using System.Text.Json;

namespace CalcAspMVC.Classes
{
    public static class SessionManager
    {
        // Session Key
        private const string SessionKey = "FloatList";

        /// <summary>
        /// Update List in User Session
        /// </summary>
        public static void SetList(this ISession session, List<float> list)
        {
            // Serialize list to JSON
            // Store JSON List in session using Key
            string jsonString = JsonSerializer.Serialize(list);
            session.SetString(SessionKey, jsonString);
        }

        /// <summary>
        /// Get List from User Session
        /// </summary>
        public static List<float> GetList(this ISession session)
        {
            // Get JSON String using SessionKey
            // Convert & Return JSON String to Float List
            string value = session.GetString(SessionKey);
            return value == null ? new List<float>() : JsonSerializer.Deserialize<List<float>>(value);
        }
    }
}
