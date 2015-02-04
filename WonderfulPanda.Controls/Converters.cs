using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WonderfulPanda.Controls
{
    public class OneWayMultiConverter<T> : IMultiValueConverter
    {
        readonly Func<object[], T> _convert;
        public OneWayMultiConverter(Func<object[], T> convert)
        {
            _convert = convert;
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return _convert(values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class DoubleToThickness : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var transform = new Thickness(1);
                if (parameter != null)
                    transform = (Thickness)new ThicknessConverter().ConvertFrom(parameter);
                var v = System.Convert.ToDouble(value);
                return new Thickness(transform.Left * v, transform.Top * v, transform.Right * v, transform.Bottom * v);
            }
            catch (FormatException)
            {
                return new Thickness(0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }

    public class DoublesToRect : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var doubles = values.Select(ToDouble).ToArray();
            switch (doubles.Length)
            {
                case 2:
                    return new Rect(0, 0, doubles[0], doubles[1]);
                case 4:
                    return new Rect(doubles[0], doubles[1], doubles[2], doubles[3]);
                default:
                    return Rect.Empty;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        double ToDouble(object value)
        {
            try
            {
                return System.Convert.ToDouble(value);
            }
            catch
            {
                return 0.0;
            }
        }
    }

    public static class Converters
    {
        public static readonly DoublesToRect DoublesToRect = new DoublesToRect();
        public static readonly DoubleToThickness DoubleToThickness = new DoubleToThickness();
    }
}
