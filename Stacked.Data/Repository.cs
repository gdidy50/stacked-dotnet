using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stacked.Data.Models;
using Stacked.Models;

namespace Stacked.Data
{
    public class Repository<T> : IRepository<T>
        where T : EntityModel
    {
        private readonly DbSet<T> _entities;
        private readonly BlogDbContext _db;

        public Repository(BlogDbContext db)
        {
            _entities = _db.Set<T>();
            _db = db;
        }

        public async Task<Guid> Create(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.CreatedOn = DateTime.UtcNow;
            entity.UpdatedOn = DateTime.UtcNow;
            _entities.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<T> GetById(Guid id) =>
            await _entities.SingleOrDefaultAsync(ent => ent.Id == id);

        public async Task<T> GetFirstWhere<TOrder>(
            Expression<Func<T, bool>> whereExp,
            Expression<Func<T, TOrder>> orderByExp) =>
            await _entities
                .Where(whereExp)
                .OrderByDescending(orderByExp)
                .FirstOrDefaultAsync();

        public async Task<List<T>> GetAllWhere<TOrder>(
            Expression<Func<T, bool>> whereExp,
            Expression<Func<T, TOrder>> orderByExp,
            int limit = 100)
        {
            var entities = _entities
                                .AsQueryable()
                                .Where(whereExp);
            return await entities
                            .Take(limit)
                            .OrderByDescending(orderByExp)
                            .ToListAsync();
        }

        public async Task<PaginationResult<T>> GetAll(int page = 1, int perPage = 3)
        {
            var count = await _entities.CountAsync();
            var entsToSkip = (page - 1) * perPage;
            var entities = await _entities
                                .OrderByDescending(ent => ent.UpdatedOn)
                                .Skip(entsToSkip)
                                .Take(perPage)
                                .ToListAsync();
            return new PaginationResult<T>
            {
                TotalCount = count,
                Results = entities,
                ResultsPerPage = perPage,
                PageNumber = page
            };
        }

        public async Task<PaginationResult<T>> GetAll(
            int page,
            int perPage,
            Expression<Func<T, bool>> whereExp)
        {
            var count = await _entities.CountAsync();
            var entsToSkip = (page - 1) * perPage;
            var entities = await _entities
                                .Where(whereExp)
                                .OrderByDescending(ent => ent.UpdatedOn)
                                .Skip(entsToSkip)
                                .Take(perPage)
                                .ToListAsync();
            return new PaginationResult<T>
            {
                TotalCount = count,
                Results = entities,
                ResultsPerPage = perPage,
                PageNumber = page
            };
        }

        public async Task<T> Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.UpdatedOn = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new ArgumentNullException(nameof(id));

                var entity = await _entities.SingleOrDefaultAsync(ent => ent.Id == id);
                _entities.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}