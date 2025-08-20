using System.Windows;
using System.Windows.Controls;

namespace POS_APP.Views.SidebarViews;
public class CategoryTemplateSelector : DataTemplateSelector
{
    public DataTemplate WithSubCategoriesTemplate { get; set; }
    public DataTemplate NoSubCategoriesTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var category = item as MenuCategory;
        if (category == null) return base.SelectTemplate(item, container);

        if (category.HasSubCategories)
            return WithSubCategoriesTemplate;
        else
            return NoSubCategoriesTemplate;
    }
}
