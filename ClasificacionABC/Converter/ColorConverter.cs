using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SiasoftAppExt
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BrushConverter ss = new BrushConverter();
            string input = value as string;

            switch (input)
            {
                case "A":return ss.ConvertFromInvariantString("#66008000");
                case "B": return ss.ConvertFromInvariantString("#7FFFFF00");
                case "C": return ss.ConvertFromInvariantString("#7FFF0000");
                default: return DependencyProperty.UnsetValue;
            }
                        
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }






    }
}
