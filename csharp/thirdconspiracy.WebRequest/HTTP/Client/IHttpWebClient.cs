using thirdconspiracy.WebRequest.HTTP.Models;

namespace thirdconspiracy.WebRequest.HTTP.Client
{
    public interface IHttpWebClient
    {
        IHttpResponseModel Execute(IHttpRequestModel httpRequest);
    }
}
