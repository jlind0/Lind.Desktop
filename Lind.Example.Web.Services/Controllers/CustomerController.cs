using Lind.Example.Data;
using Lind.Example.Data.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace Lind.Example.Web.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class RepositoryController<TEntity> : ControllerBase
        where TEntity : ExampleEntity, new()
    {
        protected IRepository<TEntity> Repository { get; }
        public RepositoryController(IRepository<TEntity> repository)
        {   
            Repository = repository;
        }
        [HttpGet("results/{page}/size={pageSize}")]
        [HttpGet("results/{page}/size={pageSize}/sort={orderBy}")]
        public async Task<RepositoryResultSet<TEntity>> Results(int page, int pageSize,string? orderBy = null, CancellationToken token = default)
        {
            
            using(var uow = Repository.CreateUnitOfWork())
            {
                int count = await Repository.Count(uow, token: token);
                Pager pager = new Pager()
                {
                    Page = page,
                    Length = pageSize
                };
                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderClause = null;
                if(orderBy != null)
                {
                    var orderData = orderBy.Split(',').Select(x => x.Split(':')).Select(x => new { ColumnName = x[0], Dsc = x[1].ToLower() == "dsc" });
                    var orderFirst = orderData.First();
                    var orderSecond = orderData.Skip(1).First();
                    orderClause = orderFirst.Dsc ? q => q.OrderByDescending(e => EF.Property<object>(e, orderFirst.ColumnName)) :
                        q => q.OrderBy(e => EF.Property<object>(e, orderFirst.ColumnName));
                    foreach(var od in orderData.Skip(1))
                    {
                       
                    }
                    
                }
                var data = await Repository.Get(uow, pager, orderBy: orderClause, token: token);
                return new RepositoryResultSet<TEntity>()
                {
                    Entities = data,
                    Count = count,
                    Page = pager.Page,
                    PageSize = pager.Length
                };
            }
            
        }
        [HttpGet("{id}")]
        public Task<TEntity?> Get(int id, CancellationToken token = default)
        {
            return Repository.GetByID(new object[] { id }, token: token);
        }
        [HttpPost]
        public async Task<TEntity> Add([FromBody] TEntity entity, CancellationToken token = default)
        {
            await Repository.Add(entity, token: token);
            return entity;
        }
        [HttpPut]
        public async Task<TEntity> Update([FromBody] TEntity entity, CancellationToken token = default)
        {
            await Repository.Update(entity, token: token);
            return entity;
        }
        [HttpDelete("{id}")]
        public Task Delete(int id, CancellationToken token = default)
        {
            return Repository.Delete(new object[] {id}, token: token);
        }
        [HttpDelete("many/{ids}")]
        public async Task Delete(string ids, CancellationToken token = default)
        {
            using (var uow = Repository.CreateUnitOfWork()) 
            {
                foreach(var id in ids.Split(',').Select(i => int.Parse(i)))
                {
                    await Repository.Delete(new object[] { id }, uow, token);
                }
                await uow.Save(token);
            }
        }
        
    }
    
    public class CustomerController : RepositoryController<Customer>
    {
        public CustomerController(IRepository<Customer> repository) : base(repository) { }  
    }
    
}
