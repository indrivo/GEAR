namespace GR.Entities.Abstractions.ViewModels.DynamicEntities
{
    public sealed class PaginationResponseViewModel
    {
        public uint Page { get; set; } = 1;
        public uint Total { get; set; } = 0;
        public uint PerPage { get; set; } = 10;
        public EntityViewModel ViewModel { get; set; }
    }
}
