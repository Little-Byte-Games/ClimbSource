using Climb.Models;

namespace Climb.ViewModels._General
{
    public class ErrorPageViewModel : BaseViewModel
    {
        public readonly int statusCode;
        public readonly string statusMessage;

        public ErrorPageViewModel(User user, int statusCode, string statusMessage)
            : base(user)
        {
            this.statusCode = statusCode;
            this.statusMessage = string.IsNullOrWhiteSpace(statusMessage) ? GetDefaultMessage(statusCode) : statusMessage;
        }

        private static string GetDefaultMessage(int statusCode)
        {
            string statusmessage;
            switch (statusCode)
            {
                case 400:
                    statusmessage = "Bad request: The request cannot be fulfilled due to bad syntax.";
                    break;
                case 403:
                    statusmessage = "Forbidden.";
                    break;
                case 404:
                    statusmessage = "Page not found.";
                    break;
                case 408:
                    statusmessage = "The server timed out waiting for the request.";
                    break;
                case 500:
                    statusmessage = "Internal Server Error - server was unable to finish processing the request.";
                    break;
                default:
                    statusmessage = "That’s odd... Something we didn't expect happened.";
                    break;
            }

            return statusmessage;
        }
    }
}
