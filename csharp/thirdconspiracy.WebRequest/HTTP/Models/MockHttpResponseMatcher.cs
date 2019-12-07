using Moq;
using System;
using System.Text.RegularExpressions;

namespace thirdconspiracy.WebRequest.HTTP.Models
{
    public class MockHttpResponseMatcher
    {
        #region Member Variables

        private readonly Regex _urlPatternToMatch;
        private readonly HttpAction _methodToMatch;
        internal readonly Mock<IHttpResponseModel> _mockResponse;

        #endregion Member Variables

        #region CTOR

        public MockHttpResponseMatcher(HttpAction methodToMatch, Regex UrlPatternToMatch, Mock<IHttpResponseModel> mockResponse)
        {
            _methodToMatch = methodToMatch;
            _urlPatternToMatch = UrlPatternToMatch;
            _mockResponse = mockResponse;
        }

        #endregion CTOR

        public bool MatchesRequest(HttpRequestModel request)
        {
            return request.Method == _methodToMatch &&
                   _urlPatternToMatch.IsMatch(request.FullUri.ToString());
        }

        public virtual IHttpResponseModel Process(HttpRequestModel request)
        {
            var now = DateTimeOffset.UtcNow;
            _mockResponse
                .Setup(m => m.CompletedAtUtc)
                .Returns(now);
            return _mockResponse.Object;
        }

    }
}
