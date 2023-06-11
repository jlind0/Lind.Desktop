using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lind.Example.Data;
using Lind.Example.Data.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Input;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;
using Lind.Example.Client.Rest;

namespace Lind.Desktop.Core.ViewModels
{
    public class MainWindowViewModel : ObservableObject
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
        protected IRepositoryClient<Customer> CustomerRepository 
            { get;  }
        public ObservableCollection<CustomerViewModel> Customers { get; } = new ObservableCollection<CustomerViewModel>();
        public IAsyncRelayCommand LoadCustomersCommand { get; }
        public IAsyncRelayCommand<int> ChangePageSizeCommand { get; }
        public IAsyncRelayCommand<int> NavigateToPageCommand { get; }
        public IAsyncRelayCommand DeleteCustomersCommand { get; }
        public MainWindowViewModel(IRepositoryClient<Customer> customerRepository)
        {
            CustomerRepository = customerRepository;
            LoadCustomersCommand = new AsyncRelayCommand(LoadCustomers);
            ChangePageSizeCommand = new AsyncRelayCommand<int>(ChangePageSize);
            NavigateToPageCommand = new AsyncRelayCommand<int>(NavigateToPage);
            DeleteCustomersCommand = new AsyncRelayCommand(DeleteCustomers);
        }
        public async Task DeleteCustomers(CancellationToken token)
        {
            var ids = Customers.Where(c => c.IsChecked).Select(c => c.CustomerId).ToArray();
            if (ids.Length > 0) {
                await CustomerRepository.Delete(ids, token);
                await LoadCustomers(token);
            }
        }
        public Task NavigateToPage(int page, CancellationToken token)
        {
            if(page <= Pages)
            {
                Page = page;
                return LoadCustomers(token);
            }    
            return Task.CompletedTask;
        }
        public Task ChangePageSize(int size, CancellationToken token)
        {
            PageSize = size;
            return LoadCustomers(token);
        }
        public async Task LoadCustomers(CancellationToken token)
        {
            Customers.Clear();
            var result = await CustomerRepository.GetAll(new Pager() { Length = PageSize, Page = Page }, 
                new OrderBy[] {
                    new OrderBy() { ColumnName = "LastName"}, 
                    new OrderBy() { ColumnName = "FirstName"}
                }, token);
            if (result != null)
            {
                Count = result.Count;
                Page = result.Page;
                PageSize = result.PageSize;
                Pages = Count / PageSize + 1;
                foreach(var r in result.Entities)
                {
                    Customers.Add(new CustomerViewModel(r, CustomerRepository));
                }
            }
        }
    }
    public class CustomerViewModel : ObservableObject
    {
        private readonly Customer Data;
        protected IRepositoryClient<Customer> CustomerRepository
        { get; }
        public CustomerViewModel(Customer data, IRepositoryClient<Customer> customerRepository)
        {
            Data = data;
            CustomerRepository = customerRepository;
        }
        public int CustomerId
        {
            get => Data.CustomerId;
            set => SetProperty(Data.CustomerId, value, 
                Data, (c, i) => c.CustomerId = i);
        }
        public bool NameStyle
        {
            get => Data.NameStyle;
            set => SetProperty(Data.NameStyle, value, 
                Data, (c, ns) => c.NameStyle = ns);
        }
        public string? Title
        {
            get => Data.Title;
            set => SetProperty(Data.Title, value,
                Data, (c, ns) => c.Title = ns);
        }
        public string FirstName
        {
            get => Data.FirstName;
            set => SetProperty(Data.FirstName, value,
                Data, (c, ns) => c.FirstName = ns);
        }
        public string? MiddleName
        {
            get => Data.MiddleName;
            set => SetProperty(Data.MiddleName, value,
                Data, (c, ns) => c.MiddleName = ns);
        }
        public string LastName
        {
            get => Data.LastName;
            set => SetProperty(Data.LastName, value,
                Data, (c, ns) => c.LastName = ns);
        }
        public string? Suffix
        {
            get => Data.Suffix;
            set => SetProperty(Data.Suffix, value,
                Data, (c, ns) => c.Suffix = ns);
        }
        public string? CompanyName
        {
            get => Data.CompanyName;
            set => SetProperty(Data.CompanyName, value,
                Data, (c, ns) => c.CompanyName = ns);
        }
        public string? SalesPerson
        {
            get => Data.SalesPerson;
            set => SetProperty(Data.SalesPerson, value,
                Data, (c, ns) => c.SalesPerson = ns);
        }
        [EmailAddress]
        public string? EmailAddress
        {
            get => Data.EmailAddress;
            set => SetProperty(Data.EmailAddress, value,
                Data, (c, ns) => c.EmailAddress = ns);
        }
        public string? Phone
        {
            get => Data.Phone;
            set => SetProperty(Data.Phone, value,
                Data, (c, ns) => c.Phone = ns);
        }
        private bool isChecked = false;
        public bool IsChecked
        {
            get => isChecked;
            set => SetProperty(ref isChecked, value);
        }
    }
}
