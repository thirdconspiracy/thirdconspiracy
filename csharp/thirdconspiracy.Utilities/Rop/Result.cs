using System;
using System.Collections.Generic;
using System.Text;

namespace thirdconspiracy.Utilities.Rop
{
    public class Result<TSuccess, TFailure>
    {
        #region Member Variables

        public bool IsSuccessful { get; set; }
        public TSuccess Success { get; set; }
        public TFailure Failure { get; set; }

        #endregion Member Variables

        #region CTOR

        public static Result<TSuccess, TFailure> Succeeded(TSuccess success)
        {
            if (success == null)
            {
                throw new ArgumentNullException(nameof(success));
            }

            return new Result<TSuccess, TFailure>
            {
                IsSuccessful = true,
                Success = success
            };
        }

        public static Result<TSuccess, TFailure> Failed(TFailure failure)
        {
            if (failure == null)
            {
                throw new ArgumentNullException(nameof(failure));
            }

            return new Result<TSuccess, TFailure>
            {
                IsSuccessful = false,
                Failure = failure
            };
        }

        private Result()
        {
        }

        #endregion CTOR

        public bool IsSuccess => IsSuccessful;
        public bool IsFailure => !IsSuccessful;
    }
}
