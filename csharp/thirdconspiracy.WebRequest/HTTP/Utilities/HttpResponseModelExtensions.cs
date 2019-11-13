using System.Net;

namespace thirdconspiracy.WebRequest.HTTP.Utilities
{
    public static class HttpResponseModelExtensions
    {
        public static bool IsSuccessCode(this HttpStatusCode statusCode)
        {
            var intStatusCode = (int)statusCode;
            return intStatusCode >= 200 && intStatusCode < 300;
        }
    }
}
