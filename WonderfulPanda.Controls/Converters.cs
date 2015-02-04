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

    public class OneWayValueConverter<T> : IValueConverter
    {

        readonly Func<object, T> _convert;
        public OneWayValueConverter(Func<object, T> convert)
        {
            _convert = convert;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return _convert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public static class Converters
    {
        public static IMultiValueConverter Rect { get; private set; }
        public static DoubleToThickness DoubleToThickness { get; private set; }

        static Converters()
        {
            Rect = new OneWayMultiConverter<Rect>(values => {
                var vals = values.Select(ToDoubleSafe).Concat(Enumerable.Repeat(0.0, 4)).Take(4).ToArray();
                return new Rect(vals[0], vals[1], vals[2], vals[3]);
            });
            DoubleToThickness = new DoubleToThickness();
        }

        static int ToIntSafe(object value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        static double ToDoubleSafe(object value)
        {
            try
            {
                return Convert.ToDouble(value);
            }
            catch
            {
                return 0.0;
            }
        }
    }
}
