using System;
using System.Collections.Generic;

namespace axionpro.application.DTOS.Pagination
{
    public class PagedResponseDTO<T>
    {
        public PagedResponseDTO()
        {
            Items = new List<T>();
        }

        public PagedResponseDTO(
            List<T> items,
            int totalCount,
            int pageNumber,
            int pageSize,
            bool? hasUploadedAll = null,
            double? completionPercentage = null)
        {
            Items = items ?? new List<T>();
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize > 0 ? pageSize : 10; // fallback if invalid

            HasUploadedAll = hasUploadedAll;
            CompletionPercentage = completionPercentage;
        }

        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }

        private int _pageSize;
        public int PageSize
        {
            get => _pageSize <= 0 ? 10 : _pageSize;
            set => _pageSize = value;
        }

        // ❌ existing logic maintain kiya gaya
        public int TotalPages = 1;

        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;

        // ✅ Nullable dynamic fields (additions)
        public bool? HasUploadedAll { get; set; }
        public double? CompletionPercentage { get; set; }
    }
}
