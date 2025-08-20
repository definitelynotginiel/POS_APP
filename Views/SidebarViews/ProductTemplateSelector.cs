using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace POS_APP.Views.SidebarViews
{
    public class ProductTemplateSelector : DataTemplateSelector
    {
        public DataTemplate WithOptionsTemplate { get; set; }
        public DataTemplate BasicTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Product product)
                return (product.HasSize || product.HasFlavor) ? WithOptionsTemplate : BasicTemplate;
            return base.SelectTemplate(item, container);
        }
    }

}
