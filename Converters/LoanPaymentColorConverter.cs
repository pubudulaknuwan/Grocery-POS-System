using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VillageSmartPOS.Converters
{
    public class LoanPaymentColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isLoanPayment)
            {
                return isLoanPayment ? Brushes.Orange : Brushes.Green;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 