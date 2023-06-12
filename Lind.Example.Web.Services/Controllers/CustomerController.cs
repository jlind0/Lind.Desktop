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
using System.Net.WebSockets;

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
        [HttpGet("results/{page}/size={pageSize}/sort={orderBy}/props={includes}")]
        [HttpGet("results/{page}/size={pageSize}/props={includes}")]
        public virtual async Task<RepositoryResultSet<TEntity>> Results(int page, int pageSize,string? orderBy = null, string? includes = null, CancellationToken token = default)
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
                List<EntityProperty> props = new List<EntityProperty>();
                if (includes != null)
                {
                    foreach (var include in includes.Split(','))
                    {
                        var i = include.Split(':');
                        props.Add(new EntityProperty(i[0], i[1].ToLower() == "col"));
                    }
                }
                var data = await Repository.Get(uow, pager, properites: 
                    includes != null ? props : null, 
                    orderBy: orderClause, token: token);
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
        [HttpGet("{id}/props={includes}")]
        public virtual Task<TEntity?> Get(int id, string? includes = null, CancellationToken token = default)
        {
            List<EntityProperty> props = new List<EntityProperty>();
            if (includes != null)
            {
                foreach (var include in includes.Split(','))
                {
                    var i = include.Split(':');
                    props.Add(new EntityProperty(i[0], i[1].ToLower() == "col"));
                }
            }
            return Repository.GetByID(new object[] { id }, properites: props, token: token);
        }
        [HttpPost]
        public virtual async Task<TEntity> Add([FromBody] TEntity entity, CancellationToken token = default)
        {
            await Repository.Add(entity, token: token);
            return entity;
        }
        [HttpPut]
        public virtual async Task<TEntity> Update([FromBody] TEntity entity, CancellationToken token = default)
        {
            await Repository.Update(entity, token: token);
            return entity;
        }
        [HttpDelete("{id}")]
        public virtual Task Delete(int id, CancellationToken token = default)
        {
            return Repository.Delete(new object[] {id}, token: token);
        }
        [HttpDelete("many/{ids}")]
        public async virtual Task Delete(string ids, CancellationToken token = default)
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
        public override async Task<Customer?> Get(int id, string? includes = null, CancellationToken token = default)
        {
            var customer = await base.Get(id, includes, token);
            if (customer != null)
            {
                foreach (var address in customer.CustomerAddresses)
                {
                    address.Customer = null;
                    if (address.Address != null)
                        address.Address.CustomerAddresses = null;
                }
            }
            return customer;
        }
        public override async Task<RepositoryResultSet<Customer>> Results(int page, int pageSize, string? orderBy = null, string? includes = null, CancellationToken token = default)
        {
            var result = await base.Results(page, pageSize, orderBy, includes, token);
            foreach(var cust in result.Entities)
            {
                foreach (var a in cust.CustomerAddresses)
                {
                    a.Customer = null;
                    if (a.Address != null)
                        a.Address.CustomerAddresses = null;
                }
            }
            return result;
        }
    }
    public class ProductController : RepositoryController<Product>
    {
        public ProductController(IRepository<Product> repository) : base(repository)
        {
        }
        
    }

}
