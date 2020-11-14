using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using thirdconspiracy.WebRequest.HTTP.Client;
using thirdconspiracy.WebRequest.HTTP.Models;

namespace thirdconspiracy.WebRequest.IntegrationTests
{
    [TestFixture]
    public class MockHttpRequestQueuedTests
    {
        [Test]
        public async Task VerifyQueuedResponse()
        {
            var mockResponse1 = new Mock<IHttpResponseModel>();
            mockResponse1
                .Setup(mr => mr.TransactionId)
                .Returns(1);

            var mockResponse2 = new Mock<IHttpResponseModel>();
            mockResponse2
                .Setup(mr => mr.TransactionId)
                .Returns(2);

            var mockWebClient = new MockHttpWebClient();
            mockWebClient.MockResponseQueue.Enqueue(mockResponse1);
            mockWebClient.MockResponseQueue.Enqueue(mockResponse2);

            var request = HttpRequestModel
                .CreateSimpleRequest(HttpAction.GET, new Uri("https://www.example.com/v1/object1"));

            // Expect Response1
            var response = await mockWebClient.Execute(request);
            Assert.AreEqual(mockResponse1.Object.TransactionId, response.TransactionId);

            // Expect Response2
            response = await mockWebClient.Execute(request);
            Assert.AreEqual(mockResponse2.Object.TransactionId, response.TransactionId);
        }
    }
}
