using Rubito.XamarinForms.SimpleAuth.Models;
using Xamarin.Forms;

namespace Rubito.XamarinForms.SimpleAuth.Tools
{
    public class AuthPersistence
    {
        private const string _usernameKey = "BearerUsername";
        private const string _userPasswordKey = "BearerUserPassword";
        private const string _userBearerToken = "BearerToken";

        public static string LoadUsername()
        {
            return LoadObject<string>(_usernameKey);
        }

        public static void StoreUsername(string name)
        {
            StoreObject(_usernameKey, name);
        }

        public static void DeleteUsername()
        {
            DeleteKey(_usernameKey);
        }

        public static string LoadPassword()
        {
            return LoadObject<string>(_userPasswordKey);
        }

        public static void StorePassword(string password)
        {
            StoreObject(_userPasswordKey, password);
        }

        public static void DeletePassword()
        {
            DeleteKey(_userPasswordKey);
        }

        public static BearerToken LoadToken()
        {
            return LoadObject<BearerToken>(_userBearerToken);
        }

        public static void StoreToken(BearerToken token)
        {
            StoreObject(_userBearerToken, token);
        }

        public static void DeleteToken()
        {
            DeleteKey(_userBearerToken);
        }

        private static void DeleteKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (Application.Current.Properties.ContainsKey(key))
                Application.Current.Properties.Remove(key);
        }

        private static T LoadObject<T>(string key) where T : class
        {
            if (Application.Current.Properties.ContainsKey(key))
            {
                return (T)Application.Current.Properties[key];
            }

            return default(T);
        }

        private static T LoadValue<T>(string key) where T : struct
        {
            if (Application.Current.Properties.ContainsKey(key))
            {
                return (T)Application.Current.Properties[key];
            }

            return default(T);
        }

        private static void StoreObject<T>(string key, T instance) where T : class
        {
            if (Application.Current.Properties.ContainsKey(key))
                Application.Current.Properties[key] = instance;
            else
                Application.Current.Properties.Add(key, instance);
        }

        private static void StoreValue<T>(string key, T value) where T : struct
        {
            if (Application.Current.Properties.ContainsKey(key))
                Application.Current.Properties[key] = value;
            else
                Application.Current.Properties.Add(key, value);
        }
    }
}
