//----------------------------------------------------------------------------------------------
// <copyright file="IndicatorSelector.cs" company="JISoft" Owner="Jaykumar K Daftary">
// MS-Pl licensed 
// </copyright>
//----------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JISoft.Pagination
{
    public class IndicatorSelector : DataTemplateSelector
    {
        public IndicatorSelector()
        {
        }

        public DataTemplate SelectedItemTemplate
        {
            get;
            set;
        }

        public DataTemplate UnSelectedItemTemplate
        {
            get;
            set;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            try
            {
                if (item != null)
                {
                    bool isSelected = (item as TemplateChooser).IsSelected;
                    if (isSelected)
                    {
                        return SelectedItemTemplate;
                    }
                    else
                    {
                        return UnSelectedItemTemplate;
                    }
                }
            }
            catch
            {
                // nothing
            }            
            return base.SelectTemplateCore(item, container);
        }

    }
}
