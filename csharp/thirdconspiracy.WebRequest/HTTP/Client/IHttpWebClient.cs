using System.Threading.Tasks;
using thirdconspiracy.WebRequest.HTTP.Models;

namespace thirdconspiracy.WebRequest.HTTP.Client
{
    public interface IHttpWebClient
    {
        Task<IHttpResponseModel> Execute(IHttpRequestModel httpRequest);
    }
}
