
using System.Net;

namespace RegistrationPeople.Application.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public IReadOnlyList<string>? Errors { get; init; }

        private ApiResponse() { }

        public static ApiResponse<T> Ok(T data)
            => new ApiResponse<T> { Data = data };

        public static ApiResponse<T> Ok()
            => new ApiResponse<T> { };

        public static ApiResponse<T> Fail(string error)
        => Fail(new List<string> { error });

        public static ApiResponse<T> Fail(IEnumerable<string> errors)
            => new ApiResponse<T>
            {
                Data = default,
                Errors = errors.ToList().AsReadOnly()
            };
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Error { get; set; }

        public static LoginResponse Fail(string error) => new() { Success = false, Error = error };
        public static LoginResponse Ok(string token) => new() { Success = true, Token = token };
    }
}
