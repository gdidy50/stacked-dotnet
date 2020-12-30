using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Stacked.Data.Models;
using Stacked.Models;

namespace Stacked.Data
{
    public interface IRepository<T>
        where T : EntityModel
    {
        Task<Guid> Create(T entity);
        Task<T> GetById(Guid id);
        Task<T> GetFirstWhere<TOrder>(
            Expression<Func<T, bool>> whereExp,
            Expression<Func<T, TOrder>> orderByExp);
        Task<List<T>> GetAllWhere<TOrder>(
            Expression<Func<T, bool>> whereExp,
            Expression<Func<T, TOrder>> orderByExp,
            int limit = 100);
        Task<PaginationResult<T>> GetAll(int page, int perPage);
        Task<PaginationResult<T>> GetAll(
            int page,
            int perPage,
            Expression<Func<T, bool>> whereExp);
        Task<T> Update(T entity);
        Task<bool> Delete(Guid id);
    }
}