namespace Shopverse.Application.Responses
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
        public object? Meta { get; set; }
        public Pagination? Pagination { get; set; }

        public bool IsSuccess => Errors == null || !Errors.Any();

        public static ApiResponse<T> Success(
            T data,
            string message = "Success",
            Pagination? pagination = null,
            object? meta = null)
        {
            return new ApiResponse<T>
            {
                Data = data,
                Message = message,
                Pagination = pagination,
                Meta = meta
            };
        }
  


        public static ApiResponse<T> Fail(string message, List<string>? errors = null)
            => new ApiResponse<T>
            {
                Message = message,
                Errors = errors ?? new List<string> { message }
            };

        public static ApiResponse<T> Fail(string message, List<string> errors, T? data = default)
        {
            return new ApiResponse<T>
            {
                Message = message,
                Errors = errors,
                Data = data
            };
        }
        public static ApiResponse<T> Fail(List<string> errors)
        {
            return new ApiResponse<T>
            {
                Message = "Validation Failed",
                Errors = errors,

            };
        }

    }

    public class Pagination
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int CurrentRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
