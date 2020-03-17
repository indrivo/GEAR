namespace GR.Core.Abstractions
{
    public interface IBase<T> : IBaseModel
    {
        T Id { get; set; }
    }
}
