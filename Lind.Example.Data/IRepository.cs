using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Lind.Example.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Lind.Example.Data
{
    public interface IContextFactory
    {
        ExampleContext CreateContext();
    }
    public class ContextFactory : IContextFactory
    {
        protected string ConnectionString { get; }
        public ContextFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public ExampleContext CreateContext()
        {
            return new ExampleContext(ConnectionString);
        }
    }
    public class UnitOfWork : IDisposable
    {
        internal ExampleContext Context { get; private set; }
        private bool disposedValue;
        public UnitOfWork(IContextFactory factory)
        {
            Context = factory.CreateContext();
        }
        
        public Task Save(CancellationToken token = default)
        {

            return Context.SaveChangesAsync(token);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
                
                disposedValue = true;
            }
        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
    public interface IRepository
    {
        UnitOfWork CreateUnitOfWork();
        Task Delete(object[] ids, UnitOfWork? work = null, CancellationToken token = default);
  
    }
    public interface IIRepository<in TEntity> : IRepository
        where TEntity: ExampleEntity, new()
    {
        Task Delete(TEntity entity, UnitOfWork? work = null, CancellationToken token = default);
        Task Add(TEntity entity, UnitOfWork? work = null,  CancellationToken token = default);
        Task Update(TEntity entity, UnitOfWork? work = null, CancellationToken token = default);
    }
    public struct EntityProperty
    {
        public string Name { get; }
        public bool IsCollection { get; }
        public EntityProperty(string name, bool isCollection = false)
        {
            Name = name;
            IsCollection = isCollection;
        }
    }
    public interface IRepository<TEntity> : IIRepository<TEntity>
        where TEntity: ExampleEntity, new()
    {
        Task<IEnumerable<TEntity>> Get(UnitOfWork? work = null,
            Pager? page = null,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            IEnumerable<EntityProperty>? properites = null,
            CancellationToken token = default);
        Task<int> Count(UnitOfWork? work = null,
            Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken token = default);
        Task<TEntity?> GetByID(object[] ids, UnitOfWork? work = null, 
            IEnumerable<EntityProperty> ? properites = null, CancellationToken token = default);
        
    }
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : ExampleEntity, new()
    {
        public virtual UnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork(ContextFactory);
        }
        protected IContextFactory ContextFactory { get; private set; }
        public Repository(IContextFactory contextFactory)
        {
            ContextFactory = contextFactory;
        }
        protected async Task Use(Func<UnitOfWork, CancellationToken, Task> worker,
            UnitOfWork? work = null, CancellationToken token = default, 
            bool saveChanges = false)
        {
            bool hasWork = work != null;
            work ??= new UnitOfWork(ContextFactory);
            try
            {
                await worker(work, token);
            }
            finally
            {
                if(!hasWork)
                {
                    if(saveChanges)
                        await work.Save(token);
                    work.Dispose();
                }
            }
        }
        public virtual Task Delete(object[] ids, UnitOfWork? work = null, CancellationToken token = default)
        {
            return Use(async (w, t) =>
            {
                TEntity? entity = await w.Context.FindAsync<TEntity>(ids, token);
                if (entity != null)
                    await Delete(entity, w, t);
            }, work, token, true);
            
        }
        public virtual Task Delete(TEntity entity, UnitOfWork? work = null, CancellationToken token = default)
        {
            return Use((w, t) =>
            {
                if (w.Context.Entry(entity).State == EntityState.Detached)
                    w.Context.Attach(entity);
                entity.IsArchived = true;
                return Task.CompletedTask;
            }, work, token, true);
        }

        public virtual async Task<IEnumerable<TEntity>> Get(UnitOfWork? work = null, 
            Pager? page = null, 
            Expression<Func<TEntity, bool>>? filter = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, 
            IEnumerable<EntityProperty>? properites = null, CancellationToken token = default)
        {
            IEnumerable<TEntity> data = Array.Empty<TEntity>();
            bool hasWork = work != null;
            work ??= new UnitOfWork(ContextFactory);
            try
            {
                await Use(async (w, t) =>
                {
                    IQueryable<TEntity> query = w.Context.Set<TEntity>();

                    if (filter != null)
                    {
                        query = query.Where(filter);
                    }
                    if (properites != null)
                    {
                        foreach (var propExp in properites.Select(e => e.Name))
                            query = query.Include(propExp);
                    }
                    if (page != null)
                    {
                        int skip = page.Value.Length * (page.Value.Page - 1);
                        int take = page.Value.Length;
                        if (orderBy != null)
                            data = await orderBy(query).Skip(skip).Take(take).ToArrayAsync(t);
                        else
                            data = await query.Skip(skip).Take(take).ToArrayAsync(t);
                    }
                    else if (orderBy != null)
                        data = await orderBy(query).ToArrayAsync(t);
                    else
                        data = await query.ToArrayAsync(t);
                }, work, token);
            }
            finally
            {
                if (!hasWork)
                    work.Dispose();
            }
            return data;
        }

        public virtual async Task<TEntity?> GetByID(object[] ids, UnitOfWork? work = null, IEnumerable<EntityProperty>? properites = null, CancellationToken token = default)
        {
            TEntity? entity = null;
            await Use(async (w, t) =>
            {
                entity = await w.Context.FindAsync<TEntity>(ids, t);
                if(entity != null && properites != null)
                    foreach(var prop in properites)
                    {

                        if (prop.IsCollection)
                            await w.Context.Entry(entity).Collection(prop.Name).LoadAsync(t);
                        else
                            await w.Context.Entry(entity).Reference(prop.Name).LoadAsync(t);
                    }
            }, work, token);
            return entity;
        }
         
        public virtual Task Add(TEntity entity, UnitOfWork? work = null, CancellationToken token = default)
        {
            return Use(async (w, t) =>
            {
                entity.ModifiedDate = DateTime.UtcNow;
                await w.Context.AddAsync(entity, t);
            }, work, token, true);
        }

        public virtual Task Update(TEntity entity, UnitOfWork? work = null, CancellationToken token = default)
        {
            return Use((w, t) =>
            {
                w.Context.Attach(entity);
                entity.ModifiedDate = DateTime.UtcNow;
                w.Context.Entry(entity).State = EntityState.Modified;
                return Task.CompletedTask;
            }, work, token, true);
        }

        public virtual async Task<int> Count(UnitOfWork? work = null, 
            Expression<Func<TEntity, bool>>? filter = null, 
            CancellationToken token = default)
        {
            int count = 0;
            await Use(async (w, t) =>
            {
                IQueryable<TEntity> query = w.Context.Set<TEntity>();
                if (filter != null)
                    query = query.Where(filter);

                count = await query.CountAsync(t);
            }, work, token);
            return count;
        }
    }
}
