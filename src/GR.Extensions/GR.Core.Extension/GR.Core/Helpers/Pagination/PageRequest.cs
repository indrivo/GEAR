
namespace GR.Core.Helpers.Pagination
{
    public class PageRequest
    {
        public bool Deleted { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool Descending { get; set; }
        public string Attribute { get; set; } = nameof(BaseModel.Created);
    }
}
