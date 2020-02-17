using System;
using System.Collections.Generic;
using System.Linq;

namespace GR.Core.Helpers.Pagination
{
    public interface IPagedResult
    {
        int CurrentPage { get; set; }
        int PageCount { get; set; }
        int PageSize { get; set; }
        int RowCount { get; set; }
    }

    public class PagedResult<T> : ResultModel<IList<T>>, IPagedResult where T : class
    {
        private static PagedResult<T> _default;
        public static PagedResult<T> DefaultResponse => _default ?? (_default = new PagedResult<T>());

        public virtual int CurrentPage { get; set; }
        public virtual int PageCount { get; set; }
        public virtual int PageSize { get; set; }
        public virtual int RowCount { get; set; }

        public virtual int FirstRowOnPage => (CurrentPage - 1) * PageSize + 1;

        public virtual int LastRowOnPage => Math.Min(CurrentPage * PageSize, RowCount);

        /// <summary>
        /// Map paged result
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual PagedResult<TDest> Map<TDest>(IEnumerable<TDest> data)
            where TDest : class
        {
            return new PagedResult<TDest>
            {
                Result = data.ToList(),
                CurrentPage = CurrentPage,
                PageCount = PageCount,
                PageSize = PageSize,
                RowCount = RowCount,
                IsSuccess = IsSuccess,
                Errors = Errors,
                KeyEntity = KeyEntity
            };
        }
    }
}
