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
using System.Windows.Input;

namespace Lind.Desktop.Core.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<TabViewModel> Tabs { get; } = new ObservableCollection<TabViewModel>();
        public IRelayCommand<TabViewModel> CloseTabCommand{ get; }
        public IRelayCommand OpenCustomersTabCommand { get; }
        public IAsyncRelayCommand<int> OpenCustomerDetailCommand { get; }
        public IRelayCommand OpenCustomerAddCommand { get; }
        private bool canOpenCustomers = true;
        public bool CanOpenCustomers
        {
            get => canOpenCustomers;
            set => SetProperty(ref canOpenCustomers, value);
        }
        private int? selectedTabIndex;
        public int? SelectedTabIndex
        {
            get => selectedTabIndex;
            set => SetProperty(ref selectedTabIndex, value);
        }
        protected IRepositoryClient<Customer> CustomersClient
            { get;  }
        
        public MainWindowViewModel(IRepositoryClient<Customer> customersClient)
        {
            CustomersClient = customersClient;
            CloseTabCommand = new RelayCommand<TabViewModel>(CloseTab);
            OpenCustomersTabCommand = new RelayCommand(OpenCustomersTab);
            OpenCustomerDetailCommand = new AsyncRelayCommand<int>(OpenCustomerDetail);
            OpenCustomerAddCommand = new RelayCommand(OpenCustomerAdd);
        }
        public async Task OpenCustomerDetail(int customerId, CancellationToken token = default)
        {
            var customer = await CustomersClient.Get(customerId, new EntityProperty[] 
                { new EntityProperty("CustomerAddresses", true)}, 
                token: token);
            Tabs.Add(new CustomerDetailTabViewModel(CustomersClient, customer));

        }
        public void OpenCustomerAdd()
        {
            Tabs.Add(new CustomerDetailTabViewModel(CustomersClient));
        }
        protected void CloseTab(TabViewModel? tabViewModel)
        {
            if (tabViewModel != null)
            {
                Tabs.Remove(tabViewModel);
                if (tabViewModel.GetType() == typeof(CustomerTabViewModel))
                    CanOpenCustomers = true;
                tabViewModel.Dispose();
            }
            SelectedTabIndex = Tabs.Count > 0 ? Tabs.Count - 1 : null;
        }
        public void OpenCustomersTab()
        {
            if (canOpenCustomers)
            {
                Tabs.Add(new CustomerTabViewModel(CustomersClient));
                canOpenCustomers = false;
                SelectedTabIndex = Tabs.Count - 1;
            }
        }
    }
    
}
