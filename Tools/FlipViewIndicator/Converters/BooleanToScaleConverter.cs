//----------------------------------------------------------------------------------------------
// <copyright file="PaginationIndicator.cs" company="JISoft" Owner="Jaykumar K Daftary">
// MS-Pl licensed 
// </copyright>
//-------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JISoft.Pagination.Converters
{
    public class BooleanToScaleConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return 1;
            }

            bool booleanValue = (bool)value;

            if (parameter != null && parameter.ToString() == "reverse")
            {
                booleanValue = !booleanValue;
            }
            if (booleanValue)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
