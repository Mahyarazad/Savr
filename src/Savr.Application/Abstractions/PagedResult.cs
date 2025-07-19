namespace Savr.Application.Abstractions
{
    public class PagedResult<T>
    {
        public long TotalCount { get; set; }
        public List<T> Items { get; set; } = new();
    }
}
