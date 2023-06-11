using Lind.Example.Data;
using Lind.Example.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Lind.Example.Client.Rest
{
    public interface IRepositoryClient 
    {
        Task Delete(int id, CancellationToken token = default);
        Task Delete(int[] ids, CancellationToken token = default);

    }
    public interface IHttpClientFactory
    {
        HttpClient Create();
    }
    public struct OrderBy
    {
        public string ColumnName { get; set; }
        public bool IsDsc { get; set; }
    }
    public class RepositoryHttpClientFactory : IHttpClientFactory
    {
        protected Uri BaseUri { get; }
        public RepositoryHttpClientFactory(Uri baseUri) {
            BaseUri = baseUri;  
        }
        public HttpClient Create()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = BaseUri;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
    public interface IRepositoryClient<TEntity> : IRepositoryClient
        where TEntity: ExampleEntity, new()
    {
        Task<TEntity?> Get(int id, CancellationToken token = default);
        Task<TEntity> Add(TEntity entity, CancellationToken token = default);
        Task<TEntity> Update(TEntity entity, CancellationToken token = default);
        Task<RepositoryResultSet<TEntity>?> GetAll(Pager page,
            IEnumerable<OrderBy>? orderBy = null, CancellationToken token = default);
    }
    public class RepositoryClient<TEntity> : IRepositoryClient<TEntity>
        where TEntity : ExampleEntity, new()
    {
        protected IHttpClientFactory ClientFactory { get; }
        public RepositoryClient(IHttpClientFactory httpClientFactory)
        {

            ClientFactory = httpClientFactory;
            
        }

        public async Task<TEntity> Add(TEntity entity, CancellationToken token = default)
        {
            using (var client = ClientFactory.Create())
            {
                var repositoryName = typeof(TEntity).Name.ToLower();
                var url = $"{repositoryName}/";
                var response = await client.PostAsJsonAsync(url, entity, token);
                if (response.IsSuccessStatusCode)
                    entity = await response.Content.ReadAsAsync<TEntity>(token);
            }
            return entity;
        }

        public async Task Delete(int id, CancellationToken token = default)
        {
            using(var client = ClientFactory.Create())
            {
                var repositoryName = typeof(TEntity).Name.ToLower();
                var url = $"{repositoryName}/{id}";
                await client.DeleteAsync(url, token);
            }
        }

        public async Task Delete(int[] ids, CancellationToken token = default)
        {
            using(var client = ClientFactory.Create())
            {
                var repositoryName = typeof(TEntity).Name.ToLower();
                var url = $"{repositoryName}/many/{string.Join(",", ids.Select(i => i.ToString()))}";
                await client.DeleteAsync(url, token);
            }
        }

        public async Task<TEntity?> Get(int id, CancellationToken token = default)
        {
            TEntity? entity = null;
            using(var client = ClientFactory.Create())
            {
                var repositoryName = typeof(TEntity).Name.ToLower();
                var url = $"{repositoryName}/{id}";
                var response = await client.GetAsync(url, token);
                if (response.IsSuccessStatusCode)
                    entity = await response.Content.ReadAsAsync<TEntity>(token);
            }
            return entity;
        }

        public async Task<RepositoryResultSet<TEntity>?> GetAll(Pager page, 
            IEnumerable<OrderBy>? orderBy = null, CancellationToken token = default)
        {
            RepositoryResultSet<TEntity>? result = null;
            using (var client = ClientFactory.Create())
            { 
                var repositoryName = typeof(TEntity).Name.ToLower();
                var url = $"{repositoryName}/results/{page.Page}/size={page.Length}";
                if (orderBy != null)
                    url += "/sort="+string.Join(",", orderBy.Select(c => $"{c.ColumnName}:{(c.IsDsc ? "dsc" : "asc")}"));
                HttpResponseMessage response = await client.GetAsync(url, token);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<RepositoryResultSet<TEntity>>(token);
                }
            }
            return result;
        }

        public async Task<TEntity> Update(TEntity entity, CancellationToken token = default)
        {
            using (var client = ClientFactory.Create())
            {
                var repositoryName = typeof(TEntity).Name.ToLower();
                var url = $"{repositoryName}/";
                var response = await client.PutAsJsonAsync(url, entity, token);
                if (response.IsSuccessStatusCode)
                    entity = await response.Content.ReadAsAsync<TEntity>();
            }
            return entity;
        }

    }
}
