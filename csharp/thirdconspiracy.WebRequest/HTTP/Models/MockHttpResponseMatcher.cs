using System;
using System.Text.RegularExpressions;

namespace thirdconspiracy.WebRequest.HTTP.Models
{
    public class MockHttpResponseMatcher
    {
        #region Member Variables

        private readonly Regex _urlPatternToMatch;
        private readonly HttpAction _methodToMatch;
        internal readonly MockHttpResponseModel _mockResponse;

        #endregion Member Variables

        #region CTOR

        public MockHttpResponseMatcher(HttpAction methodToMatch, Regex UrlPatternToMatch, MockHttpResponseModel mockResponse)
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

        public virtual MockHttpResponseModel Process(HttpRequestModel request)
        {
            _mockResponse.CompletedAtUtc = DateTime.UtcNow;
            return _mockResponse;
        }

    }
}
