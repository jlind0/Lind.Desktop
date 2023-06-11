using Lind.Desktop.Core.ViewModels;
using System;
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
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item.GetType() == typeof(CustomerTabViewModel))
                return Customer;
            throw new NotImplementedException();
        }

    }
}
