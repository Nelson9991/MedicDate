﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MedicDate.DataAccess.Repository.IRepository;
using MedicDate.Utility.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MedicDate.DataAccess.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dBSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dBSet = context.Set<TEntity>();
        }

        public async Task<TEntity> FindAsync(string id)
        {
            return await _dBSet.FindAsync(id);
        }

        public async Task<List<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            bool isTracking = true
        )
        {
            var query = EntityFetchQueryBuilder(filter, orderBy,
                includeProperties, isTracking);

            return await query.ToListAsync();
        }

        public async Task<List<TEntity>> GetAllWithPagingAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            bool isTracking = true, int pageIndex = 0, int pageSize = 10)
        {
            var query = EntityFetchQueryBuilder(filter, orderBy,
                includeProperties, isTracking);

            query = query.Paginate(pageIndex, pageSize);

            return await query.ToListAsync();
        }

        public async Task<TEntity> FirstOrDefaultAsync
        (
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null,
            bool isTracking = true
        )
        {
            var query = EntityFetchQueryBuilder(filter, null,
                includeProperties, isTracking);

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dBSet.AddAsync(entity);
        }

        public async Task<int> RemoveAsync(string id)
        {
            var resp = 1;

            var entity = await _dBSet.FindAsync(id);

            if (entity is null)
                resp = 0;
            else
                Remove(entity);

            return resp;
        }

        public void Remove(TEntity entity)
        {
            _dBSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dBSet.RemoveRange(entities);
        }

        public async Task<int> CountResourcesAsync()
        {
            return await _dBSet.CountAsync();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> ResourceExists(string resourceId)
        {
            var resourse = await FindAsync(resourceId);

            return resourse != null;
        }

        private IQueryable<TEntity> EntityFetchQueryBuilder(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null,
            bool isTracking = true)
        {
            IQueryable<TEntity> query = _dBSet;

            if (!isTracking) query = query.AsNoTracking();

            if (filter != null) query = query.Where(filter);

            if (includeProperties != null)
                query = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Aggregate(query, (current, s) => current.Include(s));

            if (orderBy != null) query = orderBy(query);

            return query;
        }
    }
}