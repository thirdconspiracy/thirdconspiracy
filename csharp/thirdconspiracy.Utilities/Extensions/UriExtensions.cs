using System;
using System.Linq;
using System.Web;

namespace thirdconspiracy.Utilities.Extensions
{
    public static class UriExtensions
    {
        public static Uri Append(this Uri uri, params string[] paths)
        {
            var url = paths.Aggregate(uri.AbsoluteUri, SafeCombine);
            return new Uri(url);
        }

        public static string GetQueryStringValue(this Uri uri, string queryStringKey)
        {
            var uriBuilder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            return query[queryStringKey];
        }

        private static string SafeCombine(string current, string path)
        {
            return $"{current.TrimEnd('/')}/{path.TrimStart('/')}";
        }
    }
}
