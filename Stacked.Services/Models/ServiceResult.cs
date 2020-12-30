using Stacked.Models;

namespace Stacked.Services.Models
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public ServiceError Error { get; set; }
    }

    public class PagedServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public PaginationResult<T> Data { get; set; }
        public ServiceError Error { get; set; }
    }

    public class ServiceError
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}