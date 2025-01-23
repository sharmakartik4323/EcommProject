using EcommProject.DataAccess.Data;
using EcommProject.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EcommProject.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;  //dbSet is variable here, dbSet T type kaa variable lee lega
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>(); //Similar to _context.T,where T = Class = Model(like CoverType,Category)
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
                query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public T Get(int id)
        {
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)// Category,CoverType
        {
            IQueryable<T> query = dbSet; //IQueryable is variable here, query is name of variable here and pass dbSet in the variable
            if (filter != null)
                query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))//Split to make array, use ',' to split array
                {
                    query = query.Include(includeProp);
                }
            }
            if (orderBy != null)
                return orderBy(query).ToList();
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Remove(int id)
        {
            dbSet.Remove(Get(id));//We can also write this dbSet.Remove(dbSet.Find(id));
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _context.ChangeTracker.Clear(); //Used to Clear all old changes
            dbSet.Update(entity);
        }

        public IQueryable<T> GetAlls(
        Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            // Apply filters
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Apply includes (eager loading)
            if (includeProperties != null)
            {
                // Split the properties

                foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            // Apply ordering
            // Apply ordering
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return query;
        }
    }
}
