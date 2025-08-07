using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VillageSmartPOS.Converters
{
    public class SearchSuggestionsVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2)
            {
                // Check if search term is empty
                bool isSearchTermEmpty = string.IsNullOrWhiteSpace(values[0]?.ToString());
                
                // Check if there are suggestions
                bool hasSuggestions = false;
                if (values[1] is int count)
                {
                    hasSuggestions = count > 0;
                }
                
                // Only show if search term is not empty AND there are suggestions
                return (!isSearchTermEmpty && hasSuggestions) ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 