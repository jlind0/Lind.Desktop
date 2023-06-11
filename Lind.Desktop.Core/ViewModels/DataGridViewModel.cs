using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lind.Example.Client.Rest;
using Lind.Example.Data;
using Lind.Example.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.Desktop.Core.ViewModels
{
    public abstract class DataGridViewModel<TRepositoryClient, TEntity, TViewModel> : ObservableObject
        where TRepositoryClient: IRepositoryClient<TEntity>
        where TEntity: ExampleEntity, new()
        where TViewModel: GridDetailViewModel<TRepositoryClient, TEntity>
    {
        private int pageSize = 20;
        public int PageSize
        {
            get => pageSize;
            set => SetProperty(ref pageSize, value);
        }
        private int page = 1;
        public int Page
        {
            get => page;
            set => SetProperty(ref page, value);
        }
        private int pages = 0;
        public int Pages
        {
            get => pages;
            set => SetProperty(ref pages, value);
        }
        private int count = 0;
        public int Count
        {
            get => count;
            set => SetProperty(ref count, value);
        }
        public virtual int[] PageOptions => new int[] { 20, 50, 100, 150, 200 };
        protected TRepositoryClient Repository
        { get; }
        public ObservableCollection<TViewModel> Data { get; } = new ObservableCollection<TViewModel>();
        public IAsyncRelayCommand LoadCommand { get; }
        public IAsyncRelayCommand<int> ChangePageSizeCommand { get; }
        public IAsyncRelayCommand<int> NavigateToPageCommand { get; }
        public IAsyncRelayCommand DeleteManyCommand { get; }
        public DataGridViewModel(TRepositoryClient customerRepository)
        {
            Repository = customerRepository;
            LoadCommand = new AsyncRelayCommand(Load);
            ChangePageSizeCommand = new AsyncRelayCommand<int>(ChangePageSize);
            NavigateToPageCommand = new AsyncRelayCommand<int>(NavigateToPage);
            DeleteManyCommand = new AsyncRelayCommand(DeleteMany);
        }
        protected virtual async Task DeleteMany(CancellationToken token)
        {
            var ids = Data.Where(c => c.IsChecked).Select(c => c.DataId).ToArray();
            if (ids.Length > 0)
            {
                await Repository.Delete(ids, token);
                await Load(token);
            }
        }
        protected virtual Task NavigateToPage(int page, CancellationToken token)
        {
            if (page <= Pages)
            {
                Page = page;
                return Load(token);
            }
            return Task.CompletedTask;
        }
        protected virtual Task ChangePageSize(int size, CancellationToken token)
        {
            PageSize = size;
            return Load(token);
        }
        protected abstract OrderBy[] GetSort();
        protected abstract TViewModel GetDetailViewModel(TEntity entity);
        protected virtual async Task Load(CancellationToken token)
        {
            Data.Clear();
            var result = await Repository.GetAll(
                new Pager() { Length = PageSize, Page = Page },
                GetSort(), token);
            if (result != null)
            {
                Count = result.Count;
                Page = result.Page;
                PageSize = result.PageSize;
                Pages = Count / PageSize + 1;
                foreach (var r in result.Entities)
                {
                    Data.Add(GetDetailViewModel(r));
                }
            }
        }
    }
    public abstract class GridDetailViewModel<TRepositoryClient, TEntity> : ObservableObject
        where TRepositoryClient: IRepositoryClient<TEntity>
        where TEntity: ExampleEntity, new()
    {
        protected readonly TEntity Data;
        protected TRepositoryClient Repository
        { get; }
        public abstract int DataId
        {
            get;
        }
        public GridDetailViewModel(TEntity data, TRepositoryClient repository)
        {
            Data = data;
            Repository = repository;
        }
        private bool isChecked = false;
        public bool IsChecked
        {
            get => isChecked;
            set => SetProperty(ref isChecked, value);
        }

    }
}
