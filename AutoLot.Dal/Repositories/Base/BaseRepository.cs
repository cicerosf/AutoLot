using System;
using System.Collections.Generic;
using System.Linq;
using AutoLot.Dal.EfStructures;
using AutoLot.Dal.Exceptions;
using AutoLot.Models.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace AutoLot.Dal.Repositories.Base
{
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity, new()
    {
        private bool _disposeContext;
        private bool _isDisposed;

        public ApplicationDbContext dbContext { get; }
        public DbSet<T> Table { get; }

        protected BaseRepository(ApplicationDbContext context)
        {
            dbContext = context;
            _disposeContext = false;
        }

        protected BaseRepository(DbContextOptions<ApplicationDbContext> options)
            : this (new ApplicationDbContext(options))
        {
            _disposeContext = true;
        }

        public virtual int Add(T entity, bool persist = true)
        {
            Table.Add(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual int AddRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.AddRange(entities);
            return persist ? SaveChanges(): 0;
        }

        public virtual int Delete(int id, byte[] timestamp, bool persist = true)
        {
            var entity = new T {Id = id, TimeStamp = timestamp };
            dbContext.Entry(entity).State = EntityState.Deleted;
            return persist ? SaveChanges() : 0;
        }

        public virtual int Delete(T entity, bool persist = true)
        {
            dbContext.Remove(entity);
            return persist ? SaveChanges(): 0;
        }

        public virtual int DeleteRange(IEnumerable<T> entities, bool persist = true)
        {
            dbContext.RemoveRange(entities);
            return persist ? SaveChanges() : 0;
        }

        public virtual void ExecureQuery(string sql, object[] SqlParametersObjects)
        {
            dbContext.Database.ExecuteSqlRaw(sql, SqlParametersObjects);
        }

        public virtual T? Find(int? id)
        {
            return Table.Find(id);
        }

        public virtual T? FindAsNoTracking(int id)
        {
            return Table.AsNoTrackingWithIdentityResolution().FirstOrDefault(e => e.Id == id);
        }

        public virtual T? FindIgnoreQueryFilters(int id)
        {
            return Table.IgnoreQueryFilters().FirstOrDefault(e => e.Id == id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return Table;
        }

        public virtual IEnumerable<T> GetAllIgnoreQueryFilters()
        {
            return Table.IgnoreQueryFilters();
        }

        public virtual int Update(T entity, bool persist = true)
        {
            Table.Update(entity);
            return persist ? dbContext.SaveChanges() : 0;
        }

        public virtual int UpdateRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.UpdateRange(entities);
            return persist ? dbContext.SaveChanges() : 0;
        }

        public virtual int SaveChanges()
        {
            try
            {
                return dbContext.SaveChanges();
            }
            catch (CustomException ex)
            {

                throw;
            }
            catch (Exception ex)
            {
                throw new CustomException("An error occurred updating the database", ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (_disposeContext)
                {
                    dbContext.Dispose();
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _isDisposed = true;
         
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~BaseRepository()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
