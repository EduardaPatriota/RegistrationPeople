
using System.Net;


namespace RegistrationPeople.Application.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public static ApiResponse<T> Ok(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new ApiResponse<T> { Success = true, Data = data, StatusCode = statusCode };
        }

        public static ApiResponse<T> Fail(string error, HttpStatusCode statusCode)
        {
            return new ApiResponse<T> { Success = false, Errors = new List<string> { error }, StatusCode = statusCode };
        }

        public static ApiResponse<T> Fails(List<string> errors, HttpStatusCode statusCode)
        {
            return new ApiResponse<T> { Success = false, Errors = errors, StatusCode = statusCode };
        }
    }
}
