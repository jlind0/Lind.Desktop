using CommunityToolkit.Mvvm.ComponentModel;
using Lind.Example.Client.Rest;
using Lind.Example.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.Desktop.Core.ViewModels
{
    public class CustomerTabViewModel : TabViewModel
    {
        protected IRepositoryClient<Customer> CustomerClient { get; }

        public override string TabTitle => "Customers";
        public GridCustomerViewModel CustomerGrid { get; }

        public CustomerTabViewModel(IRepositoryClient<Customer> customerClient)
        {
            CustomerClient = customerClient;
            CustomerGrid = new GridCustomerViewModel(customerClient);
        }
    }
    public class GridCustomerViewModel : DataGridViewModel<IRepositoryClient<Customer>, Customer, GridDetailCustomerViewModel>
    {
        public GridCustomerViewModel(IRepositoryClient<Customer> repositoryClient) : base(repositoryClient) { }

        protected override OrderBy[] GetSort()
        {
            return new OrderBy[] { new OrderBy() { ColumnName = "LastName" }, new OrderBy() { ColumnName = "FirstName" } };
        }
        protected override GridDetailCustomerViewModel GetDetailViewModel(Customer entity)
        {
            return new GridDetailCustomerViewModel(entity, Repository);
        }
    }
    public class GridDetailCustomerViewModel : GridDetailViewModel<IRepositoryClient<Customer>, Customer>
    {
        public GridDetailCustomerViewModel(Customer customer, IRepositoryClient<Customer> repositoryClient)
            : base(customer, repositoryClient) 
        {
         
        }
        public override int DataId => CustomerId;
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
       
    }
    public class CustomerDetailTabViewModel : EntityDetailTabModel<IRepositoryClient<Customer>, Customer>
    {
        public CustomerDetailTabViewModel(IRepositoryClient<Customer> repositoryClient, Customer? data = null) : base(repositoryClient, data)
        {
        }
        protected override Customer Data 
        { 
            get => base.Data;
            set 
            { 
                base.Data = value;
                OnPropertyChanged(nameof(CustomerId));
                OnPropertyChanged(nameof(NameStyle));
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(FirstName));
                OnPropertyChanged(nameof(MiddleName));
                OnPropertyChanged(nameof(LastName));
                OnPropertyChanged(nameof(Suffix));
                OnPropertyChanged(nameof(CompanyName));
                OnPropertyChanged(nameof(SalesPerson));
                OnPropertyChanged(nameof(EmailAddress));
                OnPropertyChanged(nameof(Phone));
            }
             
        }
        public override int Id => Data.CustomerId;

        public override string TabTitle => IsNew ? "Customer: New" : $"Customer: {Data.LastName},{Data.FirstName}";
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
    }
}
