using System.Net;

namespace NaughtyBunnyBot.Lovense.Exceptions
{
    public class GeneralLovenseException : Exception
    {
        public int StatusCode { get; private set; }
        public new string Message { get; private set; }

        public GeneralLovenseException(string message, int statusCode)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public GeneralLovenseException(string message, HttpStatusCode httpStatusCode)
        {
            StatusCode = Convert.ToInt32(httpStatusCode);
            Message = message;
        }
    }
}
