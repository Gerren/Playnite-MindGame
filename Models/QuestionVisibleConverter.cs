using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MindGame.Models
{
    internal class QuestionVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value = no more
            if (value == null || (!(value is bool b))) return Visibility.Visible;
            if (parameter != null && parameter is Boolean inverse && inverse) b = !b;
            if (!b) return Visibility.Visible; // I have more, therefore do not show, double negation
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
