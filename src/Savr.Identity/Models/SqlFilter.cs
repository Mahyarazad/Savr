

namespace Savr.Application.Features.Data
{
    public class SqlFilter
    {
        public string Field { get; set; } = default!;
        public string Operator { get; set; } = default!;
        public string Value { get; set; } = default!;
    }
}
