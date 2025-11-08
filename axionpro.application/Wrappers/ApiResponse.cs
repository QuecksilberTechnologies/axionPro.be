using System.Text.Json.Serialization;

namespace axionpro.application.Wrappers
{
    public class ApiResponse<T>
    {
        public ApiResponse() { }

        public ApiResponse(
            T data,
            string? message = null,
            bool isSucceeded = true,
            int? pageNumber = null,
            int? pageSize = null,
            int? totalRecords = null,
            int? totalPages = null)
        {
            IsSucceeded = isSucceeded;
            Message = message ?? string.Empty;
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = totalPages;
            Errors = new List<string>();
        }

        public bool IsSucceeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public T Data { get; set; }

        // ✅ Pagination (optional, for list responses)
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PageNumber { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PageSize { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TotalRecords { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TotalPages { get; set; }

        public static ApiResponse<T> Fail(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                IsSucceeded = false,
                Message = message,
                Data = default!,
                Errors = errors ?? new List<string> { message }
            };
        }

        public static ApiResponse<T> Success(T data, string message = "")
        {
            return new ApiResponse<T>
            {
                IsSucceeded = true,
                Message = message,
                Data = data,
                Errors = new List<string>()
            };
        }

        public static ApiResponse<T> SuccessPaginated(
            T data,
            int pageNumber,
            int pageSize,
            int totalRecords,
            int totalPages,
            string message = "")
        {
            return new ApiResponse<T>
            {
                IsSucceeded = true,
                Message = message,
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Errors = new List<string>()
            };
        }
    }
}
