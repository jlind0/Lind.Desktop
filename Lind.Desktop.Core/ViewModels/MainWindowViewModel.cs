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
        private bool canOpenCustomers = true;
        public bool CanOpenCustomers
        {
            get => canOpenCustomers;
            set => SetProperty(ref canOpenCustomers, value);
        }
        protected IRepositoryClient<Customer> CustomersClient
            { get;  }
        
        public MainWindowViewModel(IRepositoryClient<Customer> customersClient)
        {
            CustomersClient = customersClient;
            CloseTabCommand = new RelayCommand<TabViewModel>(CloseTab);
            OpenCustomersTabCommand = new RelayCommand(OpenCustomersTab);
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
        }
        public void OpenCustomersTab()
        {
            if (canOpenCustomers)
            {
                Tabs.Add(new CustomerTabViewModel(CustomersClient));
                canOpenCustomers = false;
            }
        }
    }
    
}
