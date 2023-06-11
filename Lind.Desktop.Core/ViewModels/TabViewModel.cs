using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lind.Example.Client.Rest;
using Lind.Example.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.Desktop.Core.ViewModels
{
    public abstract class TabViewModel : ObservableObject, IDisposable
    {
        private bool disposedValue;

        public abstract string TabTitle { get; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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
    public abstract class EntityDetailTabModel<TRepositoryClient, TEntity> : TabViewModel
        where TRepositoryClient: IRepositoryClient<TEntity>
        where TEntity : ExampleEntity, new()
    {
        bool isNew = false;
        public bool IsNew
        {
            get => isNew;
            set => SetProperty(ref isNew, value);
        }
        public abstract int Id { get; }
        protected TRepositoryClient RepositoryClient { get; }
        private TEntity entity;
        protected virtual TEntity Data
        {
            get => entity;
            set {
                SetProperty(ref entity, value);
                OnPropertyChanged(nameof(ModifiedDate));
            }
        }
        public IAsyncRelayCommand SaveCommand { get; }
        public IAsyncRelayCommand AddCommand { get; }
        public IAsyncRelayCommand DeleteCommand { get; }
        public EntityDetailTabModel(TRepositoryClient repositoryClient, TEntity? data = null)
        {
            IsNew = data == null;
            RepositoryClient = repositoryClient;
            entity = data ?? new TEntity();
            SaveCommand = new AsyncRelayCommand(Save);
            AddCommand = new AsyncRelayCommand(Add);
            DeleteCommand = new AsyncRelayCommand(Delete);
        }
        public virtual async Task Delete(CancellationToken token)
        {
            if(!IsNew)
                await RepositoryClient.Delete(Id, token);
        }
        public virtual async Task Save(CancellationToken token = default)
        {
            if(!IsNew)
                Data = await RepositoryClient.Update(Data, token);
        }
        public virtual async Task Add(CancellationToken token = default)
        {
            if (IsNew)
            {
                Data = await RepositoryClient.Add(Data, token);
                IsNew = false;
            }
        }
        public DateTime ModifiedDate
        {
            get => Data.ModifiedDate;
        }
    }
}
