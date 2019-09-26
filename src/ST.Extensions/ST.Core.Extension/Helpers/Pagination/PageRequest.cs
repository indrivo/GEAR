
namespace ST.Core.Helpers.Pagination
{
    public class PageRequest
    {
        public PageRequest()
        {
            Attribute = nameof(BaseModel.Created);
        }

        public bool Deleted { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool Descending { get; set; }
        public string Attribute { get; set; }
    }
}
