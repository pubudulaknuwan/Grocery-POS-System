using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using VillageSmartPOS.Models;

namespace VillageSmartPOS.Converters
{
    public class SuggestionSelectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Product selectedProduct && parameter is Product currentProduct)
            {
                if (selectedProduct.Id == currentProduct.Id)
                {
                    return new SolidColorBrush(Color.FromRgb(240, 248, 255)); // Light blue for selected
                }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 