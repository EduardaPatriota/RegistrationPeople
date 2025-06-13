
using System.Net;

namespace RegistrationPeople.Application.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public IReadOnlyList<string>? Errors { get; init; }
        public HttpStatusCode StatusCode { get; init; }

        private ApiResponse() { }

        public static ApiResponse<T> Ok(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
            => new ApiResponse<T> { Success = true, Data = data, StatusCode = statusCode };

        public static ApiResponse<T> Ok(HttpStatusCode statusCode = HttpStatusCode.OK)
            => new ApiResponse<T> { Success = true, StatusCode = statusCode };

        public static ApiResponse<T> Fail(string error, HttpStatusCode statusCode)
            => Fail(new List<string> { error }, statusCode);

        public static ApiResponse<T> Fail(IEnumerable<string> errors, HttpStatusCode statusCode)
            => new ApiResponse<T> { Success = false, Errors = errors.ToList().AsReadOnly(), StatusCode = statusCode };
    }
}
