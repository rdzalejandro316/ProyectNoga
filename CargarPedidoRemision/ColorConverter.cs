using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CargarPedidoRemision
{
    class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BrushConverter ss = new BrushConverter();
            ss.ConvertFromInvariantString("#16a085");

            string input = value as string;
            switch (input)
            {
                //Brush brush = new SolidColorBrush(#16a085);
                case "	007-10454": return Brushes.Red;
                case "D": return Brushes.Red;
                case "R": return Brushes.Red;
                case "M": return Brushes.OrangeRed;
                case "P": return ss.ConvertFromInvariantString("#16a085");

                default: return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }



    }
}
