using Lind.Desktop.Core.ViewModels;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Lind.Desktop.Client.DataTemplateSelectors
{
    public class RepositorySelector : DataTemplateSelector
    {
        public DataTemplate Customer { get; set; }
        public DataTemplate CustomerDetail { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Type t = item.GetType();
            if (t == typeof(CustomerTabViewModel))
                return Customer;
            else if (t == typeof(CustomerDetailTabViewModel))
                return CustomerDetail;
            throw new NotImplementedException();
        }

    }
}
