using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Rubito.XamarinForms.SimpleAuth.Models;
using Xamarin.Forms;

namespace Rubito.XamarinForms.SimpleAuth.Tools
{
    public static class Extensions
    {
        /// <summary>
        /// Embedds a BearerToken into the Authentication Headers.
        /// </summary>
        /// <param name="client">Target HttpClient object</param>
        /// <param name="token">Source Token</param>
        /// <returns>Client with embedded Token</returns>
        public static HttpClient EmbedToken(this HttpClient client, BearerToken token)
        {
            if (token != null)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            return client;
        }

        public static T GetBehaviour<T>(this View view) where T : Behavior<View>
        {
            return view.Behaviors?.SingleOrDefault(b => b is T) as T;
        }

        public static IEnumerable<T> GetBehaviours<T>(this View view) where T : Behavior<View>
        {
            return view.Behaviors?.Where(b => b is T) as IEnumerable<T>;
        }
    }
}
