using System;
using System.Collections.Generic;
using System.Web;

namespace thirdconspiracy.Utilities.Utilities
{
    public static class UriUtilities
    {

        public static Uri GetUri(Uri baseUri, string relativePath, Dictionary<string, string> queryParams)
        {
            var url = Combine(baseUri.ToString(), relativePath);
            var uri = new Uri(url);
            return GetFullUri(uri, queryParams);
        }

        public static Uri GetFullUri(Uri baseUri, Uri relativeUri, Dictionary<string, string> queryStrings)
        {
            var url = Combine(baseUri.ToString(), relativeUri.ToString());
            var uri = new Uri(url);
            return GetFullUri(uri, queryStrings);
        }

        public static Uri GetFullUri(Uri uri, Dictionary<string, string> queryStrings)
        {
            if (queryStrings == null || queryStrings.Count == 0)
            {
                return uri;
            }

            var uriBuilder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var qs in queryStrings)
            {
                query[qs.Key] = qs.Value;
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }

        private static string Combine(string uri1, string uri2)
        {
            uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');
            return $"{uri1}/{uri2}";
        }

    }
}
