using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Lind.Example.Data;
using Lind.Example.Data.Models;
using Lind.Desktop.Core.ViewModels;
using System.Net.Http;
using Lind.Example.Client.Rest;

namespace Lind.Desktop.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; }
        public App()
        {
            Services = ConfigureServices();
            this.InitializeComponent();
        }
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IHttpClientFactory>(sp => new RepositoryHttpClientFactory(new Uri(ConfigurationManager.AppSettings["ApiUri"])));
            services.AddScoped<IRepositoryClient<Customer>, RepositoryClient<Customer>>();
            services.AddTransient<MainWindowViewModel>();
            return services.BuildServiceProvider();
        }
    }
    
}
