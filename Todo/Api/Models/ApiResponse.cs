namespace Todo.Api.Models;

public class ApiResponse<T>
{
    public string Message { get; set; } = "Operation completed successfully";
    public T? Data { get; set; }

    public ApiResponse() { }

    public ApiResponse(T? data, string message = "Operation completed successfully")
    {
        Data = data;
        Message = message;
    }
}

public class PaginatedResponse<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public PaginationMeta Pagination { get; set; } = new();
}

public class PaginationMeta
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }

    public static PaginationMeta Create(int currentPage, int pageSize, int totalItems)
    {
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        return new PaginationMeta
        {
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasNextPage = currentPage < totalPages,
            HasPreviousPage = currentPage > 1
        };
    }
}
