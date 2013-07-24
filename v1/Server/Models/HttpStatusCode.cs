
namespace Server.Models
{
    public static class HttpStatusCode
    {
        public const int OK = 200;
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int PaymentRequired = 402;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int TooManyRequests = 429;
        public const int InternalError = 500;
        public const int NotImplemented = 501;
    }
}