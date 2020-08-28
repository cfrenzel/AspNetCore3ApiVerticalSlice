using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Api
{
    public abstract class AbstractResponse<T> where T : AbstractResponse<T>
    {
        public ResponseStatus Status { get; protected set; } = ResponseStatus.Success;

        //Error
        public string ErrorMessage { get; protected set; }
        public Exception ErrorException { get; protected set; }

        public T WithError(string responseMessage, Exception e = null)
        {
            Status = ResponseStatus.Error;
            ErrorMessage = responseMessage;
            ErrorException = e;
            return this as T;
        }


        //Failure
        public IEnumerable<string> Failures { get; protected set; }


        public T WithFailure(IEnumerable<string> failures)
        {
            _setFailure(failures);
            return this as T;
        }
        public T WithFailure(params string[] failures)
        {
            _setFailure(failures);
            return this as T;
        }

        private void _setFailure(IEnumerable<string> failures)
        {
            Status = ResponseStatus.Failure;
            Failures = failures;
        }

        public T WithFailure(FluentValidation.Results.ValidationResult fluentResult)
        {
            Status = ResponseStatus.Failure;
            this.Failures = fluentResult.Errors.Select(x => x.ErrorMessage);
            return this as T;
        }

        public string GetLoggableMessage(bool includeException = false)
        {

            StringBuilder sb = new StringBuilder();

            if (this.ErrorMessage != null)
            {
                sb.AppendFormat("Error: {0}. ", this.ErrorMessage);
            }
            if (Failures != null && Failures.Count() > 0)
            {
                foreach (var f in Failures)
                    sb.AppendFormat("Failure: {0}. ", f);
            }
            if (includeException && this.ErrorException != null)
            {
                sb.AppendFormat("Exception: {0}", this.ErrorMessage.ToString());
            }
            return sb.ToString();
        }

    }
    public enum ResponseStatus
    {
        Success = 1,
        Failure = 2, //There was a problem with the message submitted, or some pre-condition/ validation
        Error = 3, //An error occurred in processing the request, i.e. an exception was thrown
    }

}


