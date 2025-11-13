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

        // ✅ Pagination (optional)
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PageNumber { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PageSize { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TotalRecords { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TotalPages { get; set; }

        // ✅ Optional: Show only when not null
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? HasAllDocUploaded { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? CompletionPercentage { get; set; }

        // ❌ Fail Response
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

        // ✅ Success Response
        public static ApiResponse<T> Success(T data, string message = "")
        {
            return new ApiResponse<T>
            {
                IsSucceeded = true,
                Message = message,
                Data = data
            };
        }

        // ✅ Success with Pagination
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
                TotalPages = totalPages
            };
        }

        // ✅ Success with Pagination + Dynamic Completion Info
        public static ApiResponse<T> SuccessPaginatedPercentage(
            T data,
            int pageNumber,
            int pageSize,
            int totalRecords,
            int totalPages,
            string message = "",
            bool? hasUploadedAll = null,
            double? completionPercentage = null)
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
                HasAllDocUploaded = hasUploadedAll,
                CompletionPercentage = completionPercentage
            };
        }
    }
}
